using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using CsharpExcercise;

namespace OblTCPServer
{
	class Program
	{
		private static readonly List<FootballPlayer> playerLists = new List<FootballPlayer>
		{
			new FootballPlayer {ID = 1, Name = "Benjamin", Price = 200, ShirtNumber = 7},
			new FootballPlayer {ID = 2, Name = "Jeppe", Price = 100, ShirtNumber = 2}
		};
		static void Main(string[] args)
		{
			Console.WriteLine("Football player server is ready!");

			TcpListener listener = new TcpListener(IPAddress.Any, 2121);
			listener.Start();

			while (true)
			{
				TcpClient socket = listener.AcceptTcpClient();
				Console.WriteLine("New client");

				Task.Run(() =>
				{
					HandleClient(socket);
				});

			}
		}
		private static void HandleClient(TcpClient socket)
		{
			NetworkStream ns = socket.GetStream();
			StreamReader reader = new StreamReader(ns);
			StreamWriter writer = new StreamWriter(ns);

			while (true)
			{
				writer.WriteLine("Valgmuligheder for denne server:\r\nSend hentAlle, hent eller gem");
				writer.Flush();
				string message = reader.ReadLine();
				writer.WriteLine("Skriv det specifikke ID for den spiller du vil hente");
				writer.Flush();
				string messageID = reader.ReadLine();

				if (message.ToLower().StartsWith("hentalle"))
				{
					if (playerLists != null)
					{
						foreach (var players in playerLists)
						{
							writer.WriteLine($"Name: {players.Name}, Price: {players.Price}, Shirtnumber: {players.ShirtNumber}");
							writer.Flush(); 
						}
					}
				} else if (message.ToLower() == "hent")
				{
					int id = -1;
					if (int.TryParse(messageID, out id))
					{
						foreach (var players in playerLists)
						{
							if (players.ID == id)
							{
								writer.WriteLine($"Her er ID for spilleren: {players.ID}, Name: {players.Name}, Price: {players.Price}, Shirtnumber: {players.ShirtNumber}");
								writer.Flush();
							}
						}
					}
				} else if (message.ToLower().StartsWith("gem"))
				{
					FootballPlayer player = new FootballPlayer();
						playerLists.Add(player);
						player = JsonSerializer.Deserialize<FootballPlayer>(messageID);
						
						writer.WriteLine("Player saved");
						writer.Flush();
				}
				socket.Close();
			}
		}
	}
}
