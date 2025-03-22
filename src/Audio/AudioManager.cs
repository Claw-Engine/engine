using static Claw.SDL;
using Claw.Extensions;

namespace Claw.Audio;

/// <summary>
/// Representa o controle de áudios do jogo.
/// </summary>
public sealed class AudioManager : IDisposable
{
	public const int SampleRate = 48000;
	/// <summary>
	/// Máximo de efeitos sonoros simultâneos.
	/// </summary>
	public static int MaxConcurrent = 15;
	/// <summary>
	/// O quanto o volume deve aumentar/diminuir a cada sample do fade.
	/// </summary>
	public static float FadeSpeed = .01f;
	/// <summary>
	/// A distância do fim da música e do começo do fade, em segundos.
	/// </summary>
	public static float TrackDistance = 1;

	public bool PauseMusic = false;
	/// <summary>
	/// Volume geral (entre 0 e 1).
	/// </summary>
	public float MasterVolume
	{
		get => _masterVolume;
		set => _masterVolume = Mathf.Clamp(value, 0, 1);
	}
	/// <summary>
	/// Volume geral das músicas (entre 0 e 1).
	/// </summary>
	public float MusicVolume
	{
		get => _musicVolume;
		set => _musicVolume = Mathf.Clamp(value, 0, 1);
	}
	/// <summary>
	/// Evento executado quando a música é trocada.
	/// </summary>
	public event Action OnMusicChange;
	/// <summary>
	/// Evento executado quando um efeito sonoro termina, sem loop.
	/// </summary>
	public event Action<SoundEffectInstance> OnSoundEffectEnd;

	private enum Fade { None, In, Out }

	private int track;
	private Fade fade = Fade.None;
	private float fadeMultipliyer = 1, _masterVolume = 1, _musicVolume = 1;
	private float[] groupVolumes;
	private List<Music> trackList;
	private List<SoundEffectInstance> soundEffects;

	private IntPtr stream = 0;
	private SDL_AudioSpec want;
	private SDL_AudioStreamCallback callback;
	internal const SDL_AudioFormat AudioFormat = SDL_AudioFormat.SDL_AUDIO_F32LE;

	internal unsafe AudioManager()
	{
		want.freq = SampleRate;
		want.channels = 2;
		want.format = AudioFormat;

		stream = SDL_OpenAudioDeviceStream(0xFFFFFFFFu, ref want, callback, 0);

		if (stream == 0)
		{
			Console.WriteLine("Nenhuma saída de áudio encontrada!");

			return;
		}

		SDL_ResumeAudioDevice(SDL_GetAudioStreamDevice(stream));

		callback += AudioCallback;
		int groupSize = Enum.GetValues(typeof(SoundEffectGroup)).Length;
		groupVolumes = new float[groupSize];
		trackList = new List<Music>();
		soundEffects = new List<SoundEffectInstance>();

		for (int i = 0; i < groupSize; i++) groupVolumes[i] = 1;
	}
	~AudioManager() => Dispose();

	public void Dispose()
	{
		if (trackList.Count > 0)
		{
			track = trackList.Count;

			for (int i = 0; i < trackList.Count; i++) trackList[i].Dispose();

			trackList.Clear();
		}

		soundEffects.Clear();

		groupVolumes = null;

		if (stream != 0)
		{
			SDL_CloseAudioDevice(SDL_GetAudioStreamDevice(stream));

			stream = 0;
			callback = null;
		}
	}

	/// <summary>
	/// Calcula duração de um áudio.
	/// </summary>
	public static float CalculateDuration(long sampleLength, Channels channels) => (float)((double)(sampleLength / (int)channels) / SampleRate);

	/// <summary>
	/// Retorna o volume geral de um grupo.
	/// </summary>
	public float GetVolume(SoundEffectGroup group) => groupVolumes[(int)group];
	/// <summary>
	/// Altera o volume geral de um grupo.
	/// </summary>
	/// <param name="value">Entre 0 e 1.</param>
	public void SetVolume(float value, SoundEffectGroup group) => groupVolumes[(int)group] = Mathf.Clamp(value, 0, 1);

	/// <summary>
	/// Inicia/reinicia um sonoro.
	/// </summary>
	public void Play(SoundEffectInstance soundEffect)
	{
		if (soundEffect != null && soundEffects.Count < MaxConcurrent)
		{
			soundEffect.offset = 0;

			if (!soundEffects.Contains(soundEffect)) soundEffects.Add(soundEffect);
		}
	}
	/// <summary>
	/// Pausa um efeito sonoro.
	/// </summary>
	public void Stop(SoundEffectInstance soundEffect) => soundEffects.Remove(soundEffect);

	/// <summary>
	/// Adiciona uma música na lista de faixas.
	/// </summary>
	public void AddMusic(Music music) => trackList.Add(music);
	/// <summary>
	/// Pula para a próxima música.
	/// </summary>
	public void JumpMusic()
	{
		if (track == trackList.Count - 1) track = 0;
		else track++;

		ResetMusic();
		OnMusicChange?.Invoke();
	}
	/// <summary>
	/// Volta para a música anterior.
	/// </summary>
	public void BackMusic()
	{
		if (track == 0) track = trackList.Count - 1;
		else track--;

		ResetMusic();
		OnMusicChange?.Invoke();
	}
	/// <summary>
	/// Reseta a música atual.
	/// </summary>
	public void ResetMusic()
	{
		fade = Fade.None;
		fadeMultipliyer = 1;

		if (trackList.Count > track) trackList[track].ResetPosition();
	}
	/// <summary>
	/// Limpa a lista de faixas.
	/// </summary>
	public void ClearTrack()
	{
		track = 0;
		fade = Fade.None;
		fadeMultipliyer = 1;

		trackList.Clear();
	}
	/// <summary>
	/// Retorna a música atual.
	/// </summary>
	public Music CurrentMusic()
	{
		if (trackList.Count == 0) return null;

		return trackList[track];
	}

	/// <summary>
	/// Callback de manuseio do buffer de áudio.
	/// </summary>
	private unsafe void AudioCallback(IntPtr userdata, IntPtr stream, int additional_amount, int total_amount)
	{
		if (additional_amount > 0)
		{
			byte* data = stackalloc byte[additional_amount];
			
			AudioCallback((void*)userdata, data, additional_amount);
			SDL_PutAudioStreamData(stream, data, additional_amount);
		}
	}
	/// <summary>
	/// Callback de manuseio do buffer de áudio.
	/// </summary>
	private unsafe void AudioCallback(void* userData, byte* stream, int length)
	{
		length /= 4;
		float* buffer = (float*)stream;
		bool finished;
		Music music = trackList.Count > track ? trackList[track] : null;
		SoundEffectInstance current;

		for (int i = 0; i < length; i += 2)
		{
			buffer[i] = 0;
			buffer[i + 1] = 0;

			if (!PauseMusic && music != null)
			{
				switch (fade)
				{
					case Fade.None:
						if (trackList.Count > 1 && music.Current >= music.Duration - TrackDistance)
						{
							fade = Fade.Out;

							goto case Fade.Out;
						}

						break;
					case Fade.Out:
						fadeMultipliyer = Math.Max(fadeMultipliyer - Math.Abs(FadeSpeed), 0);

						if (fadeMultipliyer == 0)
						{
							JumpMusic();

							fade = Fade.In;
							music = trackList[track];
						}
						break;
					case Fade.In:
						fadeMultipliyer = Math.Min(fadeMultipliyer + Math.Abs(FadeSpeed), 1);
						
						if (fadeMultipliyer == 1) fade = Fade.None;
						break;
				}

				SetSample(buffer, i, music.GetSample(out bool ended), (_musicVolume * fadeMultipliyer), music.Channels);

				if (music.Channels == Channels.Stereo) SetSample(buffer, i + 1, music.GetSample(out ended), (_musicVolume * fadeMultipliyer), 0);

				if (ended && fade == Fade.Out)
				{
					JumpMusic();

					fade = Fade.In;
				}
			}

			for (int j = soundEffects.Count - 1; j >= 0; j--)
			{
				current = soundEffects[j];
				
				if (current != null)
				{
					SetSample(buffer, i, current.GetSample(out finished), groupVolumes[(int)current.Group], current, true);
					
					if (!finished && current.audio.Channels == Channels.Stereo) SetSample(buffer, i + 1, current.GetSample(out finished), groupVolumes[(int)current.Group], current, false);

					if (finished && !current.IsLooped)
					{
						OnSoundEffectEnd?.Invoke(current);
						RemoveAt(soundEffects, j);
					}
				}
			}
		}
	}
	/// <summary>
	/// <para>Faz a mixagem de um sample no buffer.</para>
	/// <para>Se o sample for mono, então divide o som dele entre os dois lados.</para>
	/// </summary>
	private unsafe void SetSample(float* buffer, int index, float sample, float volume, Channels channels)
	{
		if (sample == 0) return;

		sample *= _masterVolume * volume;

		if (channels == Channels.Mono)
		{
			sample *= .5f;
			buffer[index + 1] = Mathf.Clamp(buffer[index + 1] + sample, -1, 1);
		}

		buffer[index] = Mathf.Clamp(buffer[index] + sample, -1, 1);
	}
	/// <summary>
	/// <para>Faz a mixagem de um sample no buffer.</para>
	/// <para>Se o sample for mono, então divide o som dele entre os dois lados.</para>
	/// </summary>
	private unsafe void SetSample(float* buffer, int index, float sample, float volume, SoundEffectInstance sfx, bool isLeft)
	{
		if (sample == 0) return;

		sample *= _masterVolume * volume;
		
		switch (sfx.audio.Channels)
		{
			case Channels.Mono:
				sample *= .5f;
				buffer[index] = Mathf.Clamp((buffer[index] + sample) * sfx.LeftVolume, -1, 1);
				buffer[index + 1] = Mathf.Clamp((buffer[index + 1] + sample) * sfx.RightVolume, -1, 1);
				break;
			case Channels.Stereo:
				if (isLeft) buffer[index] = Mathf.Clamp((buffer[index] + sample) * sfx.LeftVolume, -1, 1);
				else buffer[index] = Mathf.Clamp((buffer[index] + sample) * sfx.RightVolume, -1, 1);
				break;
		}
	}
	/// <summary>
	/// Remove um item da lista, sem se preocupar com a ordem.
	/// </summary>
	private static void RemoveAt<T>(List<T> list, int index)
	{
		list.Swap(index, list.Count - 1);
		list.RemoveAt(list.Count - 1);
	}
}
