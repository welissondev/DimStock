﻿using DimStock.Views;
using DimStock.Models;

namespace DimStock.Presenters
{
    public class DestinationListingPresenter
    {
        private IDestinationListingView view;

        public DestinationListingPresenter(IDestinationListingView view)
        {
            this.view = view;
        }

        public bool Delete()
        {
            var actionResult = false;

            if (new DestinationModel() { Id = view.Id }.Delete() == true)
            {
                actionResult = true;
                Clear();
            }

            return actionResult;
        }

        public bool GetDetails()
        {
            var actionResult = false;

            var destination = new DestinationModel() { Id = view.Id };

            if (destination.GetDetails() == true)
            {
                view.LocationDescription = destination.LocationDescription;
                actionResult = true;
            }

            return actionResult;
        }

        public void Clear()
        {
            view.Id = 0;
            view.SearchLocationDescription = string.Empty;
            view.LocationDescription = string.Empty;

            SearchData();
        }

        public void SearchData()
        {
            var destination = new DestinationModel()
            {
                LocationDescription = view.SearchLocationDescription
            };

            view.DataList = destination.SearchData();
        }
    }
}
