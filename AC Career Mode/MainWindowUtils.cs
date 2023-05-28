using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utilities;
using DBLink;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Numerics;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using DBLink.Classes;
using DBLink.Services;

#pragma warning disable CS8605 // Unboxing a possibly null value.

/// This file contains some population functions for the main window
/// Originally it was all in the same file but it cluttered reading
/// So now instead, it calls these functions
/// Most don't have a return because they interact with variables directly in the other file (at the top)

namespace AC_Career_Mode
{


    public partial class MainWindow : Window
    {
        #region POPULATE
        private void PopulateCarMarket(bool SerializeCurrent)
        {
            // Called when a car was bought
            if (SerializeCurrent) Utils.ProtoSerialize(MarketCars, GlobalVars.CarMarketBin);

            MarketCars.Clear();

            // Using a seed so cars are changed daily
            for (int i = 0; i < 25; i++)
            {
                int index = RandomDaily.Next(CarsSource.Count);
                MarketCars.Add(CarsSource[index]);
            }

            // If serialized current, this will load the list srlzd at the beginning
            MarketCars = Utils.ReadWriteBin(GlobalVars.CarMarketBin, MarketCars);

            uc_MarketCars.CarList = MarketCars;
        }

        private void PopulateTrackMarket(bool SerializeCurrent)
        {
            // Called when a track was bought
            if (SerializeCurrent) Utils.ProtoSerialize(MarketTracks, GlobalVars.TrackMarketBin);

            // Using a seed so tracks are changed daily
            for (int i = 0; i < 5; i++)
            {
                int index = RandomDaily.Next(TracksSource.Count);
                MarketTracks.Add(TracksSource[index]);
            }

            // If serialized current, this will load the list srlzd at the beginning
            MarketTracks = Utils.ReadWriteBin(GlobalVars.TrackMarketBin, MarketTracks);

            uc_MarketTracks.TrackList = MarketTracks;
        }

        private void PopulateLoans(bool SerializeCurrent)
        {
            // Called when a loan was taken
            if (SerializeCurrent) Utils.ProtoSerialize(LoanSource, GlobalVars.LoanMarketBin);

            uc_AvailableLoans.LoansLst = null;

            for (int i = 0; i < 10; i++)
            {
                LoanSource.Add(new Loan(i));
            }

            LoanSource = Utils.ReadWriteBin(GlobalVars.LoanMarketBin, LoanSource);

            uc_AvailableLoans.LoansLst = LoanSource;
        }

        private void PopulateRaceList(bool SerializeCurrent)
        {
            // Called when a race was raced
            if (SerializeCurrent) Utils.ProtoSerialize(RaceSource, GlobalVars.RacesBin);


            // Create 200 random races each day
            for (int i = 0; i < 200; i++)
            {
                Race race = Race.RaceFromList(TracksSource, CarsSource, i);
                RaceSource.Add(race);
            }

            RaceSource = Utils.ReadWriteBin(GlobalVars.RacesBin, RaceSource);

            uc_RaceTab.RaceList = RaceSource;
            uc_RaceTab.CurrentUser = CurrentUser;

        }

        private void UpdateAndRefreshPlayer(Player profile)
        {
            profile.PayDueLoans();
            profile.PayRevenue();
            profile.UpdateInDB();

            OwnedCars_lv.Cars = new ObservableCollection<Car>(profile.GetPlayerCars());
            

            List<Driver?> drivers = new List<Driver?>();
            for (int i = 0; i < 10; i++)
            {
                drivers.Add(AIDriverData.GenAIDriver(i));
            }

            OwnedCars_lv.Drivers = new ObservableCollection<Driver?>(new Driver[] { null, profile }.Concat(drivers));


            // Update UI
            profile = Player.LoadFromDB(profile.Id);
            
            toplabel_User.Content = profile.Name;
            toplabel_Money.Content = profile.Money.ToString("##,#");
            toplabel_Wins.Content = $"🏆 {profile.RaceWins}";
            toplabel_Races.Content = $"Races: {profile.Races}";


            uc_PlayerLoans.LoansLst = profile.GetPlayerLoans();
            uc_PlayerTracks.TrackList = profile.GetPlayerTracks();
            uc_Records.Records =  Record.DeserializeRecords(profile);

            // DB returns null as 0
            if (profile.EquippedCarId != 0)
            {
                toplabel_EquippedCar.Content = Car.LoadCar(profile.EquippedCarId).Name;
            }
            else
            {
                toplabel_EquippedCar.Content = "";
            }
        }
        #endregion

        private void GetAvailableCarsAndTracks()
        {
            TracksSource.Clear();
            CarsSource.Clear();

            /// Race track exceptions
            string[] InvalidTracks = { "drag", "drift", "indoor karting", "yatabe" };

            foreach (string ui_track in Directory.GetFiles(GlobalVars.TRACKS_PATH, "ui_track.json", SearchOption.AllDirectories))
            {
                Track track = Track.LoadTrackJson(ui_track);

                if (InvalidTracks.Any(InvalidTrack => track.Name.ToLower().Contains(InvalidTrack)))
                { continue; }


                DirectoryInfo ui_folder = Directory.GetParent(ui_track);
                track.OutlinePath = ui_folder.ToString() + @"\" + "outline.png";

                TracksSource.Add(track);
            }

            foreach (string ui_car in Directory.GetFiles(GlobalVars.CARS_PATH, "ui_car.json", SearchOption.AllDirectories))
            {
                Car car = Car.LoadCarJson(ui_car);
                CarsSource.Add(car);
            }

            TracksSource = Utils.ReadWriteBin(GlobalVars.TracksBin, TracksSource);
            CarsSource = Utils.ReadWriteBin(GlobalVars.CarsBin, CarsSource);
        }


    }

}
