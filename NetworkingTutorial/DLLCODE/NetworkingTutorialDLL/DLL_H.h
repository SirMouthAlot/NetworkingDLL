#pragma once

#include <Windows.h>
#include "Server.h"

#ifndef DLL_OUT
#define DLL_OUT __declspec(dllexport)
#endif

// Wrapper begins
extern "C"
{
	static Client client;
	static Server server;

	bool client_started = false;
	bool server_started = false;

	// Need to transfer these to Unity
	DLL_OUT void InitDLL(bool console, CS_to_Plugin_Functions functions);
	DLL_OUT void InitServer(const char* IP, const int port);
	DLL_OUT void InitClient(const char* IP, const int port, const char* name);
	DLL_OUT void Cleanup();

	DLL_OUT void SendPacketToServer(const char* message);

	void InitConsole();
	void ClearConsole();
}
