#define _CRT_SECURE_NO_WARNINGS

#pragma once

#include "CPPtoCSFunctionContainer.h"
#include <WS2tcpip.h>
#include <string>
#include <thread>
#include <iostream>
#include <vector>
#include <map>
#include <unordered_map>

#pragma comment(lib, "ws2_32.lib")

struct ClientConnectionData
{
	const char* name;
	const char* status;
	int id;

	sockaddr_in addr;

	const char* to_message()
	{
		std::string msg = "";
		msg += "c";
		msg += ";";
		msg += name;
		msg += ";";
		msg += status;
		msg += ";";
		msg += std::to_string(id);
		
		return msg.c_str();
	}
};

class Client
{
public:
	void Init(const char* IP, const int port, const char* name, bool wsa);
	void ConnectToServer();
	void SendPacketToServer(const char* message);
	void Cleanup();

	CS_to_Plugin_Functions funcs;

private:
// Functions
	void StartListen();
	void RecvLoop();
	void ProcessMessage(const char* msgIn);

// Values
	const char* clientName;
	const char* IP;
	int port;

	bool listening = false;
	HANDLE threadHandle;

	SOCKET sock;
	sockaddr_in serverAddr;
};