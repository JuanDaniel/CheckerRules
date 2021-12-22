﻿using Autodesk.Revit.DB;
using SpreadsheetLight;
using System;

namespace BBI.JD
{
    public interface ICheckerRule
    {
        Guid Id { get; }
        string Name { get; }
        string Group { get; }
        void Execute(Document document, SLDocument excel);
    }
}