using LiveChartsCore.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xstrat.Models.Supabase;

namespace xstrat.Core
{
    public static class DataCache
    {
        #region Team
        public static Team _currentTeam;
        public static Team CurrentTeam
        {
            get
            {
                if (_currentTeam == null)
                {
                    RetrieveTeam();
                }
                return _currentTeam;
            }
            set
            {
                _currentTeam = value;
            }
        }

        public static void RetrieveTeam()
        {
            var task = ApiHandler.GetTeamInfoAsync();
            task.Wait();
            CurrentTeam = task.Result;
        }
        #endregion

        #region User
        public static UserData _currentUser;
        public static UserData CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    RetrieveUser();
                }
                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        }

        public static void RetrieveUser()
        {
            var task = ApiHandler.GetUserDataAsync();
            task.Wait();
            CurrentUser = task.Result;
        }
        #endregion

        #region Teammates
        public static List<UserData> _currentTeamMates;
        public static List<UserData> CurrentTeamMates
        {
            get
            {
                if (_currentTeamMates == null)
                {
                    RetrieveTeamMates().Wait();
                }
                return _currentTeamMates;
            }
            set
            {
                _currentTeamMates = value;
            }
        }

        public static async Task RetrieveTeamMates()
        {
            var task = ApiHandler.GetTeamMembersAsync();
            task.Wait();
            CurrentTeamMates = task.Result.EmptyIfNull();
        }
        #endregion

        #region Maps
        public static List<Map> _currentMaps;
        public static List<Map> CurrentMaps
        {
            get
            {
                if (_currentMaps == null)
                {
                    RetrieveMaps();
                }
                return _currentMaps;
            }
            set
            {
                _currentMaps = value;
            }
        }

        public static void RetrieveMaps()
        {
            var task = ApiHandler.GetMapsAsync();
            task.Wait();
            CurrentMaps = task.Result.EmptyIfNull();
        }
        #endregion

        #region Operators
        public static List<Operator> _currentOperators;
        public static List<Operator> CurrentOperators
        {
            get
            {
                if (_currentOperators == null)
                {
                    RetrieveOperators();
                }
                return _currentOperators;
            }
            set
            {
                _currentOperators = value;
            }
        }

        public static void RetrieveOperators()
        {
            var task = ApiHandler.GetOperatorsAsync();
            task.Wait();
            CurrentOperators = task.Result.EmptyIfNull();
        }
        #endregion

        #region Games
        public static List<Game> _currentGames;
        public static List<Game> CurrentGames
        {
            get
            {
                if (_currentGames == null)
                {
                    RetrieveGames();
                }
                return _currentGames;
            }
            set
            {
                _currentGames = value;
            }
        }

        public static void RetrieveGames()
        {
            var task = ApiHandler.GetGamesAsync();
            task.Wait();
            CurrentGames = task.Result.EmptyIfNull();
        }
        #endregion

        #region Positions
        public static List<Position> _currentPositions;
        public static List<Position> CurrentPositions
        {
            get
            {
                if (_currentPositions == null)
                {
                    RetrievePositions();
                }
                return _currentPositions;
            }
            set
            {
                _currentPositions = value;
            }
        }

        public static void RetrievePositions()
        {
            var task = ApiHandler.GetPositionsAsync();
            task.Wait();
            CurrentPositions = task.Result.EmptyIfNull();
        }
        #endregion

        #region Routines
        public static List<Routine> _currentRoutines;
        public static List<Routine> CurrentRoutines
        {
            get
            {
                if (_currentRoutines == null)
                {
                    RetrieveRoutines();
                }
                return _currentRoutines;
            }
            set
            {
                _currentRoutines = value;
            }
        }

        public static void RetrieveRoutines()
        {
            var task = ApiHandler.GetRoutinesAsync();
            task.Wait();
            CurrentRoutines = task.Result.EmptyIfNull();
        }
        #endregion

        #region CalendarBlocks
        public static List<CalendarBlock> _currentCalendarBlocks;
        public static List<CalendarBlock> CurrentCalendarBlocks
        {
            get
            {
                if (_currentCalendarBlocks == null)
                {
                    RetrieveCalendarBlocks();
                }
                return _currentCalendarBlocks;
            }
            set
            {
                _currentCalendarBlocks = value;
            }
        }

        public static void RetrieveCalendarBlocks()
        {
            var task = ApiHandler.GetTeamOffDays();
            task.Wait();
            CurrentCalendarBlocks = task.Result.EmptyIfNull();
        }
        #endregion

        #region CalendarEvents
        public static List<CalendarEvent> _currentCalendarEvents;
        public static List<CalendarEvent> CurrentCalendarEvents
        {
            get
            {
                if (_currentCalendarEvents == null)
                {
                    RetrieveCalendarEvents();
                }
                return _currentCalendarEvents;
            }
            set
            {
                _currentCalendarEvents = value;
            }
        }

        public static void RetrieveCalendarEvents()
        {
            var task = ApiHandler.GetTeamScrims();
            task.Wait();
            CurrentCalendarEvents = task.Result.EmptyIfNull();
        }
        #endregion

        #region Strats
        public static List<Strat> _currentStrats;
        public static List<Strat> CurrentStrats
        {
            get
            {
                if (_currentStrats == null)
                {
                    RetrieveStrats();
                }
                return _currentStrats;
            }
            set
            {
                _currentStrats = value;
            }
        }

        public static void RetrieveStrats()
        {
            var task = ApiHandler.GetStrats();
            task.Wait();
            CurrentStrats = task.Result.EmptyIfNull();
        }
        #endregion


        #region CalendarEventResponses
        public static List<CalendarEventResponse> _currentCalendarEventResponses;
        public static List<CalendarEventResponse> CurrentCalendarEventResponses
        {
            get
            {
                if (_currentCalendarEventResponses == null)
                {
                    RetrieveCalendarEventResponses();
                }
                return _currentCalendarEventResponses;
            }
            set
            {
                _currentCalendarEventResponses = value;
            }
        }

        public static void RetrieveCalendarEventResponses()
        {
            var task = ApiHandler.GetCalendarEventResponsesAsync();
            task.Wait();
            CurrentCalendarEventResponses = task.Result.EmptyIfNull();
        }
        #endregion
    }
}
