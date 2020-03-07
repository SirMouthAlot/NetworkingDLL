#include "pch.h"
#include "DLL_H.h"

DLL_OUT void InitDLL(bool console, CS_to_Plugin_Functions functions)
{
	if (console)
		InitConsole();

	// Pass in functions here
	client.funcs = functions;
}

void InitConsole()
{
	ClearConsole();
	FILE* pConsole;
	AllocConsole();
	freopen_s(&pConsole, "CONOUT$", "wb", stdout);

	std::cout << "Welcome to our DLL!.\n";
	std::cout << "==========================================\n";
}

void ClearConsole()
{
	COORD tl = { 0,0 };
	CONSOLE_SCREEN_BUFFER_INFO s;
	HANDLE console = GetStdHandle(STD_OUTPUT_HANDLE);
	GetConsoleScreenBufferInfo(console, &s);
	DWORD written, cells = s.dwSize.X * s.dwSize.Y;
	FillConsoleOutputCharacter(console, ' ', cells, tl, &written);
	FillConsoleOutputAttribute(console, s.wAttributes, cells, tl, &written);
	SetConsoleCursorPosition(console, tl);
}

DLL_OUT void InitServer(const char* IP, const int port)
{
	server_started = true;
	server.InitServer(IP, port);
}

// Needs to take in IP and port
DLL_OUT void InitClient(const char* IP, const int port, const char* name)
{
	client_started = true;
	client.Init(IP, port, name, server_started);
}

DLL_OUT void SendPacketToServer(const char* message)
{
	client.SendPacketToServer(message);
}

DLL_OUT void Cleanup()
{
	WSACleanup();

	if (client_started){
		client.Cleanup();
	}
	if (server_started){
		server.Cleanup();
	}

	// If you need to NUKE the program, use this
	//ExitProcess(0);
}