﻿namespace EcommerceDDD.Core.Exceptions;

public class ApplicationDataException : Exception
{
    public ApplicationDataException(string message) : base(message) { }
}