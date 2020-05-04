using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class FishInfoReader
{
    private void Stuff()
    {
        
        using(var reader = new StreamReader(@"C:\test.csv"))
        {
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                listA.Add(values[0]);
                listB.Add(values[1]);
            }
        }
    }
}