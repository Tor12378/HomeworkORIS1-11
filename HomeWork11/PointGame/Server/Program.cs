﻿using Server;

internal class Program
{
    private static void Main(string[] args)
    {
        var server = new ServerObject();
        server.Listen();
    }
}