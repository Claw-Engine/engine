#include "Clawdio.h"

const int SOUND_START = 7; // INT32:SAMPLERATE|Byte:CHANNELS|Ushort:SAMPLENUMBER

template<typename T>
T read_value(std::ifstream& file)
{
	T value;

	file.read(reinterpret_cast<char*>(&value), sizeof(value));

	return value;
}
int* read_samples(std::ifstream& file, int size)
{
	static int* samples = new int[size];

	file.read(reinterpret_cast<char*>(samples), sizeof(int) * size);

	return samples;
}

int min(int a, int b)
{
	if (a < b) return a;

	return b;
}

void audio_callback(void* userData, Uint8* stream, int length)
{
	AudioBase* base = (AudioBase*)userData;

	base->audio_callback(stream, length);
}

AudioBase::AudioBase(int sampleRate, Uint8 channels, Uint16 length) : SampleRate(sampleRate), Channels(channels), Length(length), Volume(1) { }

SoundEffect::SoundEffect(int sampleRate, Uint8 channels, int* samples, Uint16 length) : AudioBase(sampleRate, channels, length), Samples(samples) { }

void SoundEffect::audio_callback(Uint8* stream, int length)//Temp
{
	int* buffer = (int*)stream;
	int bufferSize = min(length / 4, Length);

	for (int i = 0; i < bufferSize; i++) buffer[i] = Samples[i];
}

Music::Music(int sampleRate, Uint8 channels, std::ifstream* file, Uint16 length) : AudioBase(sampleRate, channels, length), File(file) { }

void Music::audio_callback(Uint8* stream, int length)//Temp
{
	int* buffer = (int*)stream;
	int bufferSize = min(length / 4, Length);
	int* samples = read_samples(*File, bufferSize);

	for (int i = 0; i < bufferSize; i++) buffer[i] = samples[i];

	if (File->tellg() >= SOUND_START + Length * sizeof(int)) File->seekg(SOUND_START);
}

AudioBase* read_audio(char* path, bool isMusic)
{
	static AudioBase* result = nullptr;
	static std::ifstream file(path, std::ios::binary);

	if (file.is_open())
	{
		int sampleRate = read_value<int>(file);
		Uint8 channels = read_value<Uint8>(file);
		Uint16 length = read_value<Uint16>(file);

		if (isMusic) result = &(Music(sampleRate, channels, &file, length));
		else
		{
			static int* samples = read_samples(file, length);
			result = &(SoundEffect(sampleRate, channels, samples, length));

			file.close();
		}
	}

	return result;
}

void set_want(SDL_AudioSpec& want, AudioBase* audio)
{
	want.freq = (*audio).SampleRate;
	want.format = AUDIO_S32LSB;
	want.channels = (*audio).Channels;
	want.samples = (*audio).Length;
	want.userdata = audio;
	want.callback = audio_callback;
}