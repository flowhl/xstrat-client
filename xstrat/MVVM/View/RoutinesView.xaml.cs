using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xstrat.Core;
using xstrat.Ui;
using xstrat.Overlay;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for RoutinesView.xaml
    /// </summary>
    public partial class RoutinesView : UserControl
    {
        private List<Routine> routines = new List<Routine>();
        private List<RoutineStep> routineSteps = new List<RoutineStep>();
        string trennzeichen_step = "#%&#";
        string trennzeichen_para = "#§$#";
        private int currentRoutine_id;
        private string currentRoutine_title;
        private RoutineOverlay overlay;
        public RoutinesView()
        {
            InitializeComponent();
            Retrieve();
            UpdateUI();
        }

        private void addRoutines()
        {
            for (int i = 0; i < 5; i++)
            {
                routineSteps.Add( newRoutineStep("Title", "Description", 1, 60));
            }
            for (int i = 0; i < 5; i++)
            {
                routines.Add(newRoutine("Title", "", 123));
            }
        }

        /// <summary>
        /// Returns new Routine
        /// </summary>
        /// <param name="header"></param>
        /// <param name="createdDate"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private Routine newRoutine(string header, string createdDate, int id)
        {
            var newRoutine = new Routine(header, createdDate, id);
            newRoutine.MoveButtonEvent += new EventHandler<RoutineButtonClicked>(MoveButtonEvent);
            return newRoutine;
        }

        private RoutineStep newRoutineStep(string header, string body, int count, int duration)
        {
            var newRoutineStep = new RoutineStep(header, body, count, duration);
            newRoutineStep.MoveButtonEvent += new EventHandler<RoutineStepButtonClicked>(MoveStepButtonEvent);
            return newRoutineStep;
        }

        #region Drag n Drop

        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();

        private void sp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == this.sp)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(sp);
            }
        }

        private void sp_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
            if(_realDragSource != null)
            {
                _realDragSource.ReleaseMouseCapture();
            }
            
        }

        private void sp_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this.sp).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this.sp).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    _isDragging = true;
                    _realDragSource = e.Source as UIElement;
                    _realDragSource.CaptureMouse();
                    DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                }
            }
        }

        private void sp_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void sp_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                UIElement droptarget = e.Source as UIElement;
                int droptargetIndex = -1, i = 0;
                foreach (UIElement element in this.sp.Children)
                {
                    if (element.Equals(droptarget))
                    {
                        droptargetIndex = i;
                        break;
                    }
                    i++;
                }
                if (droptargetIndex != -1)
                {

                    this.sp.Children.Remove(_realDragSource);
                    this.sp.Children.Insert(droptargetIndex, _realDragSource);
                    MirrorSP();
                }

                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();
            }
        }

        private void MirrorSP()
        {
            routineSteps.Clear();
            foreach (var item in sp.Children)
            {
                if(item.GetType() == typeof(RoutineStep))
                {
                    routineSteps.Add((RoutineStep)item);
                }
            }
            UpdateUI();
        }


        #endregion
        private void MoveStepButtonEvent(object sender, RoutineStepButtonClicked e)
        {
            /// 1- move down
            /// 1 - move up
            /// 2 - remove
            /// 3 - add
            
            int Type = e.Type;
            RoutineStep routine = e.Instance;
            int currentIndex = routineSteps.IndexOf(routine);
            if (Type == 1 || Type == -1)
            {
                int newIndex = currentIndex - Type;
                if (newIndex >= 0 && newIndex <= routineSteps.Count - 1)
                {
                    routineSteps.RemoveAt(currentIndex);
                    routineSteps.Insert(newIndex, routine);
                }
            }
            if (Type == 2)
            {
                routineSteps.RemoveAt(currentIndex);
            }
            if(Type == 3)
            {
                routineSteps.Add( newRoutineStep("Title", "Description", 1, 60));
            }
            
            UpdateUI();
        }

        private void MoveButtonEvent(object sender, RoutineButtonClicked e)
        {
            /// 0 - open
            /// -1 - remove
            if (e.Type == -1)
            {
                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you really want to delete this Routine? This cannot be reverted", "Delete Routine?", System.Windows.Forms.MessageBoxButtons.YesNo);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    int index = routines.IndexOf(e.Instance);
                    APIdeleteRoutineAsync(routines[index].ID);
                    routines.RemoveAt(index);
                }
                Retrieve();
                UpdateUI();
            }
            if (e.Type == 0)
            {
                RetrieveSteps(e.Instance.ID, e.Instance.Head);
            }
            
        }

        private async void Retrieve()
        {
            (bool, string) result = await ApiHandler.GetAllRoutines();
            if (result.Item1)
            {
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                if(data != null && data != "")
                {
                    List<xstrat.Json.Routine> rList = JsonConvert.DeserializeObject<List<Json.Routine>>(data);
                    routines.Clear();
                    foreach (var r in rList)
                    {
                        routines.Add(newRoutine(r.title, r.created_date, r.id));
                    }
                }
                else
                {
                    Notify.sendError("Routines could not be created");
                    throw new Exception("Routines could not be created");
                }
                UpdateUI();
            }
            else
            {
                return;
            }
        }

        private async void RetrieveSteps(int r_id, string r_title)
        {
            currentRoutine_id = r_id;
            currentRoutine_title = r_title;
            (bool, string) result = await ApiHandler.GetRoutineContent(r_id);
            if (result.Item1)
            {
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                List<xstrat.Json.Content> _content = JsonConvert.DeserializeObject<List<xstrat.Json.Content>>(data);
                string content = _content[0].content;
                contentToList(content);
                if(routineSteps.Count < 1)
                {
                    routineSteps.Add(newRoutineStep("Title", "Description", 1, 60));
                }
                UpdateUI();
            }

        }

        private void UpdateUI()
        {
            sp.Children.Clear();
            RoutinesSP.Children.Clear();
            foreach (var item in routineSteps)
            {
                sp.Children.Add(item);
            }
            foreach (var item in routines)
            {
                RoutinesSP.Children.Add(item);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var r in routines)
            {
                APIrenameRoutine(r.Head, r.ID);
            }
            if(routines.Count > 0 && routines.Where(x => x.ID == currentRoutine_id).Any())
            {
                var routine = routines.Where(x => x.ID == currentRoutine_id).First();
                if (routine != null)
                {
                    APIsaveRoutine(routine.Head, listToContent(), currentRoutine_id);
                }
            }
            
        }

        private void OpenOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(overlay != null)
            {
                overlay.Close();
            }
            overlay = new RoutineOverlay(currentRoutine_title);
            var overlaySteps = new List<OverlayStep>();
            foreach (var step in routineSteps)
            {
                OverlayStep newstep = new OverlayStep(step.Duration, step.Count, step.Header, false, false);
                overlaySteps.Add(newstep);
            }
            if(overlaySteps.Count > 0)
            {
                overlaySteps[0].IsSelected = true;
                overlay.Initialize(overlaySteps);
                overlay.Show();
                overlay.Top = 0;
                overlay.Left = 0;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            APInewRoutineAsync();
        }

        private void contentToList(string content)
        {
            routineSteps.Clear();
            if (content != null && content != "")
            {
                var steps = content.Split( new string[] { trennzeichen_step }, StringSplitOptions.None);
                foreach (var step in steps)
                {
                    if(step != null && step != "")
                    {
                        var stepparts = step.Split(new string[] { trennzeichen_para }, StringSplitOptions.None);
                        routineSteps.Add(newRoutineStep(stepparts[0], stepparts[1], int.Parse(stepparts[2]), int.Parse(stepparts[3])));
                    }
                }
            }
        }

        private string listToContent()
        {
            string result = "";
            foreach (var item in routineSteps)
            {
                result += trennzeichen_step;
                result += item.Header;
                result += trennzeichen_para;
                result += item.Body;
                result += trennzeichen_para;
                result += item.Count.ToString();
                result += trennzeichen_para;
                result += item.Duration.ToString();
            }
            return result;
        }


        private async Task APInewRoutineAsync()
        {
            (bool, string) result = await ApiHandler.NewRoutine();
            if (result.Item1)
            {
                Retrieve();
            }
            else
            {
                Notify.sendError("Could not save new routine: " + result.Item2);
                //MessageBox.Show("Could not save Routine: " + result.Item2);
            }
        }
        private async Task APIdeleteRoutineAsync(int id)
        {
            if(id >= 0)
            {
                (bool, string) result = await ApiHandler.DeleteRoutine(id);
                if (result.Item1)
                {
                    Notify.sendSuccess("Successfully saved");
                    return;
                }
                else
                {
                    Notify.sendError("Could not delete routine: " + result.Item2);
                    //MessageBox.Show("Could not delete Routine: " + result.Item2);
                }
            }
        }
        private async void APIsaveRoutine(string title, string content, int routine_id)
        {
            if(title != null && content != null && title != "" && content != "")
            {
                (bool, string) result = await ApiHandler.SaveRoutine(title, content, routine_id);
                if (result.Item1)
                {
                    Notify.sendSuccess("Successfully saved");
                    Retrieve();
                }
                else
                {
                    Notify.sendError("Could not save routine: " + result.Item2);
                    //MessageBox.Show("Could not save Routine: " + result.Item2);
                }
            }
            else
            {
                Notify.sendError( "Could not save Routine: Content or title is empty");
                //MessageBox.Show("Could not save Routine. content or title is empty");
            }
        }

        private async void APIrenameRoutine(string title, int routine_id)
        {
            if (title != null && title != "")
            {
                (bool, string) result = await ApiHandler.RenameRoutine(title, routine_id);
                if (result.Item1)
                {
                    //Notify.sendSuccess("Success", "Successfully saved");
                    return;
                }
                else
                {
                    Notify.sendError("Could not rename routine: " + result.Item2);
                    //MessageBox.Show("Could not rename Routine: " + result.Item2);
                }
            }
            else
            {

                Notify.sendError("Could not save Routine: Content or title is empty");
                //MessageBox.Show("Could not save Routine. content or title is empty");
            }
        }

    }
}
