﻿namespace TaskManagement.Application.Helpers.GenerateJwt;

public static class RandomGenerator
{
    public static int GenerateInteger(int min, int max)
    {
        return new Random().Next(min, max);
    }
}
