using ExchangeService.Models.Database;
using ExchangeService.Models.DTOs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExchangeService.ExtensionMethods
{
  public static class ExtensionMethods
  {
    public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
    {
      foreach (var element in toAdd)
      {
        @this.Add(element);
      }
    }

    public static ExchangeRateViewModel ToViewModel(this ExchangeRate @this)
    {
      return new ExchangeRateViewModel(@this);
    }

    public static void TraceException(this Exception _this)
    {
      Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, _this);
    }
  }
}