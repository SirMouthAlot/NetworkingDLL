#include "pch.h"
#include "Client.h"

#define CLIENTLOG std::cout << "Client: "

void Client::Init(const char* p_IP, const int p_port, const char* name, bool wsaisinit)
{
	WSAData data;
	WORD version = MAKEWORD(2, 2);

	if (!wsaisinit)
	{
		if (WSAStartup(version, &data) != 0)
		{
			CLIENTLOG << "Error Initializing winsock\n";
			return;
		}
	}

	CLIENTLOG << "Init Complete\n";

	IP = p_IP;
	port = p_port;

	clientName = name;

	ConnectToServer();
}

void Client::ConnectToServer()
{
	serverAddr.sin_addr.S_un.S_addr = INADDR_BROADCAST;
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(port);
	inet_pton(AF_INET, IP, &serverAddr.sin_addr);

	sock = socket(AF_INET, SOCK_DGRAM, 0);
	if (sock == INVALID_SOCKET)
	{
		CLIENTLOG << "Socket was invalid\n";
		return;
	}

	StartListen();

	SendPacketToServer(clientName);
}

void Client::SendPacketToServer(const char* msg)
{
	CLIENTLOG << "Sending: " << msg << std::endl;

	if (sendto(sock, msg, std::string(msg).length(), 0, (sockaddr*)&serverAddr, sizeof(serverAddr)) == SOCKET_ERROR)
	{
		CLIENTLOG << "ERR: Could not send msg to server.\n";
		return;
	}
}

void Client::Cleanup()
{
	listening = false;
	closesocket(sock);
}

void Client::StartListen()
{
	listening = true;
	std::thread(&Client::RecvLoop, this).detach();
}

void Client::RecvLoop()
{
	int sizeofServer = sizeof(serverAddr);
	char buf[256];

	while (listening)
	{
		if (!listening)
			return;

		ZeroMemory(buf, 256);

		int numBytesReceived = recvfrom(sock, buf, 256, 0, (sockaddr*)&serverAddr, &sizeofServer);

		if (numBytesReceived > 0)
		{
			ProcessMessage(buf);
		}
	}
}

void Client::ProcessMessage(const char* msg)
{
	CLIENTLOG << "Client received: " << msg << std::endl;
	funcs.MsgReceived(msg);
}
