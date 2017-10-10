using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ConferenceScheduler.Entities;
using ConferenceScheduler.Interfaces;

namespace HybridConferenceScheduler
{
    [TestClass]
    public class BasicExamplesDemo
    {
        [TestMethod]
        public void ScheduleWithNoRestrictions()
        {
            bool disableTrace = true;

            var engine = (null as IConferenceOptimizer).Create(disableTrace);
            var sessions = new SessionsCollection();
            var rooms = new List<Room>();
            var timeslots = new List<Timeslot>();


            // Presenters
            var presenterJustinJames = Presenter.Create(10, "Justin James");
            var presenterWendySteinman = Presenter.Create(12, "Wendy Steinman");
            var presenterHattanShobokshi = Presenter.Create(16, "Hattan Shobokshi");
            var presenterRyanMilbourne = Presenter.Create(22, "Ryan Milbourne");
            var presenterMaxNodland = Presenter.Create(23, "Max Nodland");
            var presenterBarryStahl = Presenter.Create(24, "Barry Stahl");
            var presenterJustineCocci = Presenter.Create(25, "Justine Cocci");
            var presenterChrisGriffith = Presenter.Create(26, "Chris Griffith");
            var presenterIotLaboratory = Presenter.Create(27, "IOT Laboratory");
            var presenterScottGu = Presenter.Create(28, "Scott Guthrie");
            var presenterScottHanselman = Presenter.Create(29, "Scott Hanselman");
            var presenterDamianEdwards = Presenter.Create(30, "Damian Edwards");


            // Sessions
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


            // Session dependencies
            sessionBlockchain101.AddDependency(sessionBitcoin101);


            // Timeslots
            timeslots.Add(Timeslot.Create(1, 9.5));
            timeslots.Add(Timeslot.Create(2, 11));
            timeslots.Add(Timeslot.Create(3, 13));
            timeslots.Add(Timeslot.Create(4, 14.5));


            // Rooms
            rooms.Add(Room.Create(1, 10)); // Unex 127
            rooms.Add(Room.Create(2, 10)); // Unex 126
            rooms.Add(Room.Create(3, 10, new int[] { 3, 4 })); // Unex 110  -- Only available in AM
            rooms.Add(Room.Create(4, 10)); // Unex 107
            rooms.Add(Room.Create(5, 10)); // Unex 106


            // Create the schedule
            var assignments = engine.Process(sessions, rooms, timeslots);

            // Display the results
            assignments.WriteSchedule(sessions);

        }

        [TestMethod]
        public void ScheduleWithHardConstraints()
        {
            bool disableTrace = true;

            var engine = (null as IConferenceOptimizer).Create(disableTrace);
            var sessions = new SessionsCollection();
            var rooms = new List<Room>();
            var timeslots = new List<Timeslot>();


            // Presenters

            // No restrictions on when they present
            var presenterJustinJames = Presenter.Create(10, "Justin James");
            var presenterWendySteinman = Presenter.Create(12, "Wendy Steinman");
            var presenterHattanShobokshi = Presenter.Create(16, "Hattan Shobokshi");
            var presenterRyanMilbourne = Presenter.Create(22, "Ryan Milbourne");
            var presenterIotLaboratory = Presenter.Create(27, "IOT Laboratory");

            // Prefers the last 2 sessions of the day
            var presenterMaxNodland = Presenter.Create(23, "Max Nodland", new int[] { 1, 2 });

            // Doesn't like morning sessions
            var presenterBarryStahl = Presenter.Create(24, "Barry Stahl", new int[] { 1, 2 });
            #region A Possible fix
            // var presenterBarryStahl = Presenter.Create(24, "Barry Stahl", new int[] { 1 });
            #endregion

            // Hates 1st session of morning & afternoon
            var presenterJustineCocci = Presenter.Create(25, "Justine Cocci", new int[] { 1, 3 });

            // Prefers 1st session of the day
            var presenterChrisGriffith = Presenter.Create(26, "Chris Griffith", new int[] { 2, 3, 4 });

            // Flying in and out so only available during the middle of the day
            var presenterScottGu = Presenter.Create(28, "Scott Guthrie", new int[] { 1, 4 });

            // Flying in together so only available toward the end of the day
            var presenterScottHanselman = Presenter.Create(29, "Scott Hanselman", new int[] { 1, 2 });
            var presenterDamianEdwards = Presenter.Create(30, "Damian Edwards", new int[] { 1, 2 });



            // Sessions
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

            // Session dependencies
            sessionBlockchain101.AddDependency(sessionBitcoin101);


            // Timeslots
            timeslots.Add(Timeslot.Create(1, 9.5));
            timeslots.Add(Timeslot.Create(2, 11));
            timeslots.Add(Timeslot.Create(3, 13));
            timeslots.Add(Timeslot.Create(4, 14.5));


            // Rooms
            rooms.Add(Room.Create(1, 10)); // Unex 127
            rooms.Add(Room.Create(2, 10)); // Unex 126
            rooms.Add(Room.Create(3, 10, new int[] { 3, 4 })); // Unex 110  -- Only available in AM
            rooms.Add(Room.Create(4, 10)); // Unex 107
            rooms.Add(Room.Create(5, 10)); // Unex 106


            // Create the schedule
            var assignments = engine.Process(sessions, rooms, timeslots);

            // Display the results
            assignments.WriteSchedule(sessions);

        }

        [TestMethod]
        public void ScheduleWithTimePreferencesAsAnOptimization()
        {
            bool disableTrace = true;

            var engine = (null as IConferenceOptimizer).Create(disableTrace);
            var sessions = new SessionsCollection();
            var rooms = new List<Room>();
            var timeslots = new List<Timeslot>();


            // Presenters
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



            // Sessions
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

            // Session dependencies
            sessionBlockchain101.AddDependency(sessionBitcoin101);



            // Timeslots
            timeslots.Add(Timeslot.Create(1, 9.5));
            timeslots.Add(Timeslot.Create(2, 11));
            timeslots.Add(Timeslot.Create(3, 13));
            timeslots.Add(Timeslot.Create(4, 14.5));


            // Rooms
            rooms.Add(Room.Create(1, 10)); // Unex 127
            rooms.Add(Room.Create(2, 10)); // Unex 126
            rooms.Add(Room.Create(3, 10, new int[] { 3, 4 })); // Unex 110  -- Only available in AM
            rooms.Add(Room.Create(4, 10)); // Unex 107
            rooms.Add(Room.Create(5, 10)); // Unex 106


            // Create the schedule
            var assignments = engine.Process(sessions, rooms, timeslots);

            // Display the results
            assignments.WriteSchedule(sessions);

        }
    }
}
