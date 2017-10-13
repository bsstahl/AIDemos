using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ConferenceScheduler.Entities;
using ConferenceScheduler.Interfaces;
using ConferenceScheduler.Exceptions;

namespace HybridConferenceScheduler
{
    [TestClass]
    public class AddConstraintsDemo
    {
        [TestMethod]
        public void ScheduleByAddingConstraints()
        {
            bool disableTrace = true;

            var engine = (null as IConferenceOptimizer).Create(disableTrace);
            var sessions = new SessionsCollection();
            var rooms = new List<Room>();
            var timeslots = new List<Timeslot>();


            #region Presenters
            // No restrictions on when they present
            var presenterJustinJames = Presenter.Create(10, "Justin James");
            var presenterWendySteinman = Presenter.Create(12, "Wendy Steinman");
            var presenterHattanShobokshi = Presenter.Create(16, "Hattan Shobokshi");
            var presenterRyanMilbourne = Presenter.Create(22, "Ryan Milbourne");
            var presenterIotLaboratory = Presenter.Create(27, "IOT Laboratory");

            // Prefers the last 2 sessions of the day
            var preferredTimeslotsMaxNodland = new int[] { 3, 4 };
            var presenterMaxNodland = Presenter.Create(23, "Max Nodland", new int[] { }, preferredTimeslotsMaxNodland);

            // Doesn't like morning sessions
            var preferredTimeslotsBarryStahl = new int[] { 3, 4 };
            var presenterBarryStahl = Presenter.Create(24, "Barry Stahl", new int[] { }, preferredTimeslotsBarryStahl);

            // Doesn't like 1st session of morning & afternoon
            var preferredTimeslotsJustineCocci = new int[] { 2, 4 };
            var presenterJustineCocci = Presenter.Create(25, "Justine Cocci", new int[] { }, preferredTimeslotsJustineCocci);

            // Prefers 1st session of the day
            var preferredTimeslotsChrisGriffith = new int[] { 1 };
            var presenterChrisGriffith = Presenter.Create(26, "Chris Griffith", new int[] { }, preferredTimeslotsChrisGriffith);

            // Flying in and out so only available during the middle of the day
            var unavailableTimeslotsScottGu = new int[] { 1, 4 };
            var presenterScottGu = Presenter.Create(28, "Scott Guthrie", unavailableTimeslotsScottGu);

            // Flying in together so only available toward the end of the day
            var unavailableTimeslotsScottHanselman = new int[] { 1, 2 };
            var presenterScottHanselman = Presenter.Create(29, "Scott Hanselman", unavailableTimeslotsScottHanselman);
            var unavailableTimeslotsDamianEdwards = new int[] { 1, 2 };
            var presenterDamianEdwards = Presenter.Create(30, "Damian Edwards", unavailableTimeslotsDamianEdwards);

            #endregion

            #region Sessions
            var sessionPublicSpeaking = sessions.Add(12, "Everyone is Public Speaker", (int)Topic.None, presenterJustinJames);
            var sessionTimeyWimey = sessions.Add(14, "Timey-Wimey Stuff", null, presenterWendySteinman);
            var sessionBitcoin101 = sessions.Add(24, "Bitcoin 101", (int)Topic.None, presenterRyanMilbourne);
            var sessionBlockchain101 = sessions.Add(25, "Blockchain 101", (int)Topic.None, presenterRyanMilbourne);
            var sessionRapidRESTDev = sessions.Add(26, "Rapid REST Dev w/Node & Sails", (int)Topic.None, presenterJustinJames);
            var sessionNativeMobileDev = sessions.Add(27, "Native Mobile Dev With TACO", (int)Topic.None, presenterJustinJames);
            var sessionReduxIntro = sessions.Add(28, "Redux:Introduction", (int)Topic.None, presenterMaxNodland);
            var sessionReactGettingStarted = sessions.Add(29, "React:Getting Started", (int)Topic.None, presenterMaxNodland);
            var sessionDevSurveyOfAI = sessions.Add(30, "Devs Survey of AI", (int)Topic.None, presenterBarryStahl);
            var sessionMLIntro = sessions.Add(31, "ML:Intro to Image & Text Analysis", (int)Topic.None, presenterJustineCocci);
            var sessionChatbotsIntroInNode = sessions.Add(32, "ChatBots:Intro using Node", (int)Topic.None, presenterJustineCocci);
            var sessionAccidentalDevOps = sessions.Add(33, "Accidental DevOps:CI for .NET", (int)Topic.None, presenterHattanShobokshi);
            var sessionWhatIsIonic = sessions.Add(34, "What is Ionic", (int)Topic.None, presenterChrisGriffith);

            var sessionEverythingCloud = sessions.Add(41, "Everything about Cloud", (int)Topic.None, presenterScottGu);
            var sessionFunnyMobileDev = sessions.Add(42, "Funny Mobile Development", (int)Topic.None, presenterScottHanselman);
            var sessionMobileForNerdz = sessions.Add(43, "Mobile for Nerdz", (int)Topic.None, presenterScottHanselman);
            var sessionDotNetCoreAwesomeness = sessions.Add(44, ".NET Core Awesomeness", (int)Topic.None, presenterDamianEdwards);
            var sessionDotNetStandard20 = sessions.Add(45, ".NET Standard 2.0", (int)Topic.None, presenterDamianEdwards);

            #endregion

            #region Session dependencies

            sessionBlockchain101.AddDependency(sessionBitcoin101);

            #endregion

            #region Timeslots

            timeslots.Add(Timeslot.Create(1, 9.5));
            timeslots.Add(Timeslot.Create(2, 11));
            timeslots.Add(Timeslot.Create(3, 13));
            timeslots.Add(Timeslot.Create(4, 14.5));

            #endregion

            #region Rooms

            rooms.Add(Room.Create(1, 10)); // Unex 127
            rooms.Add(Room.Create(2, 10)); // Unex 126
            rooms.Add(Room.Create(3, 10, new int[] { 3, 4 })); // Unex 110  -- Only available in AM
            rooms.Add(Room.Create(4, 10)); // Unex 107
            rooms.Add(Room.Create(5, 10)); // Unex 106

            #endregion

            #region Timeslots out-of-favor for each presenter

            var presenterUnfavoredTimeslots = new List<Tuple<Presenter, int>>();

            // This list is built separately so it maintains the order
            // in which the requests were submitted.  It could also be built
            // by negating the list of preferred timeslots for each presenter

            // Prefers the last 2 sessions of the day
            presenterUnfavoredTimeslots.Add(presenterMaxNodland, 1);
            presenterUnfavoredTimeslots.Add(presenterMaxNodland, 2);

            // Doesn't like morning sessions
            presenterUnfavoredTimeslots.Add(presenterBarryStahl, 1);
            presenterUnfavoredTimeslots.Add(presenterBarryStahl, 2);

            // Doesn't like 1st session of morning & afternoon
            presenterUnfavoredTimeslots.Add(presenterJustineCocci, 1);
            presenterUnfavoredTimeslots.Add(presenterJustineCocci, 3);

            // Prefers 1st session of the day
            presenterUnfavoredTimeslots.Add(presenterChrisGriffith, 2);
            presenterUnfavoredTimeslots.Add(presenterChrisGriffith, 3);
            presenterUnfavoredTimeslots.Add(presenterChrisGriffith, 4);

            #endregion

            #region Create the schedule

            // Note: We want this to throw an exception here if it is infeasible 
            // since this is the least restrictive it could ever be.
            IEnumerable<Assignment> assignments = engine.Process(sessions, rooms, timeslots);
            var lastSuccessfulAssignments = assignments.Clone();

            foreach (var unfavoredTimeslot in presenterUnfavoredTimeslots)
            {
                int currentPresenterId = unfavoredTimeslot.Item1.Id;
                var currentPresenter = sessions.First(s => s.Presenters.Any(p => p.Id == currentPresenterId)).Presenters.First(p => p.Id == currentPresenterId);
                int unfavoredTimeslotId = unfavoredTimeslot.Item2;

                try
                {
                    currentPresenter.UnavailableForTimeslots = currentPresenter.UnavailableForTimeslots.Add(unfavoredTimeslotId);
                    assignments = engine.Process(sessions, rooms, timeslots);
                    lastSuccessfulAssignments = assignments;
                    Console.WriteLine($"Successfully prevented assignment of {unfavoredTimeslot.Item1.Name} to Timeslot {unfavoredTimeslotId}");
                }
                catch (NoFeasibleSolutionsException nfs)
                {
                    Console.WriteLine($"Unable to prevent assignment of {unfavoredTimeslot.Item1.Name} to Timeslot {unfavoredTimeslotId}");
                    lastSuccessfulAssignments.WriteSchedule(sessions);
                    Console.WriteLine();
                    currentPresenter.UnavailableForTimeslots = currentPresenter.UnavailableForTimeslots.Remove(unfavoredTimeslotId);
                }
            }

            #endregion

            // Display the results
            lastSuccessfulAssignments.WriteSchedule(sessions);

        }
    }
}
