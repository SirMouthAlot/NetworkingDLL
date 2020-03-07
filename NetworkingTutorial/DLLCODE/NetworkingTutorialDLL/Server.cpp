#include "pch.h"
#include "Server.h"

#define SERVERLOG std::cout<<"Server: "

void Server::InitServer(const char* p_IP, const int p_port)
{
}

void Server::Cleanup()
{
}

int Server::InitWinsock()
{
}

void Server::StartListen()
{
}

int Server::InitSocket()
{
}

int Server::BindSocket()
{
}

void Server::RecvLoop()
{
}

void Server::ProcessMessage(const char* msg, sockaddr_in clientAddr)
{
}

bool Server::IsNewUser(sockaddr_in clientAddr)
{
}

void Server::SendPacketToTargetClient(const char* msg, const sockaddr_in clientAddr)
{
}

// Send a message to all clients except for the one specified
void Server::BroadcastMessage(const char* msg, const sockaddr_in clientAddrExcept)
{
}

ClientConnectionData* Server::FindClientByAddress(sockaddr_in addr)
{
}


