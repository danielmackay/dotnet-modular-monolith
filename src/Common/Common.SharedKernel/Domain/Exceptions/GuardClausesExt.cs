﻿// using Ardalis.GuardClauses;
// using System.Runtime.CompilerServices;
//
// namespace Common.SharedKernel.Domain.Exceptions;
//
// public static class FooGuard
// {
//     public static void ZeroOrNegative(this IGuardClause guardClause,
//         int input,
//         [CallerArgumentExpression("input")] string? parameterName = null)
//     {
//         if (input <= 0)
//             throw new ArgumentException("Cannot be zero or negative", parameterName);
//     }
//
//     public static void ZeroOrNegative(this IGuardClause guardClause,
//         decimal input,
//         [CallerArgumentExpression("input")] string? parameterName = null)
//     {
//         if (input <= 0)
//             throw new ArgumentException("Cannot be zero or negative", parameterName);
//     }
// }
