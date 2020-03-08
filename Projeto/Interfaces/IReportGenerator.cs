﻿using System.Collections.Generic;

namespace DimStock.Interfaces
{
    public interface IReportGenerator<T>
    {
        void GenerateReport(List<T> list);
    }
}