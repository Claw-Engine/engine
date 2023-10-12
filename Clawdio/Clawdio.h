#pragma once
#include <fstream>
#include <SDL.h>

struct AudioBase
{
public:
	int SampleRate;
	Uint8 Channels;
	Uint16 Length;
	float Volume;

	AudioBase(int sampleRate, Uint8 channels, Uint16 length);

	virtual void audio_callback(Uint8* stream, int length) { }
};

struct SoundEffect : AudioBase
{
public:
	int* Samples;

	SoundEffect(int sampleRate, Uint8 channels, int* samples, Uint16 length);

	void audio_callback(Uint8* stream, int length);
};

struct Music : AudioBase
{
public:
	std::ifstream* File;

	Music(int sampleRate, Uint8 channels, std::ifstream* file, Uint16 length);

	void audio_callback(Uint8* stream, int length);
};

extern "C"
{
	/// <summary>Carrega um áudio.</summary>
	__declspec(dllexport) AudioBase* read_audio(char* path, bool isMusic);
	/// <summary>Seta os valores de um SDL_AudioSpec, com base num áudio.</summary>
	__declspec(dllexport) void set_want(SDL_AudioSpec& want, AudioBase* audio);
}