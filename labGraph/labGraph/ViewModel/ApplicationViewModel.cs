using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Essy.Tools.InputBox;
using Microsoft.Win32;
using labGraph.ViewModel;
using labGraph.Model.InterfaceImplementation;
using System.Windows.Media;

namespace labGraph
{


    public class ApplicationViewModel : BaseAplicationViewModel
    {
        #region private field
        enum SelectedAction
        {
            IsCursor = 1,
            IsMove,
            IsVertex,
            IsEdge,
            IsDelete
        }

        private Graph _graph = new Graph();
        private readonly Canvas _canvas;
        private Vertex _selectedVertex1, _selectedVertex2;
        private SelectedAction _selectedAction;
        private List<Graph> _undoHistoryList;
        private List<Graph> _redoHistoryList;
        private string _pathSaveFile = "";
        #endregion

        public ApplicationViewModel(Canvas canvas)
        {

            _canvas = canvas;
            _selectedAction = SelectedAction.IsCursor;
            _undoHistoryList = new List<Graph>();
            _redoHistoryList = new List<Graph>();
            History.Add("Старт программы: " + DateTime.Now.ToString());
        }

        #region collection for mvvm
        public DataTable Matrix
        {
            get
            {
                return _graph.AdjacencyMatrix;
            }

        }
        public void EditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var str = e.EditingElement.ToString().Split(' ');
            string value;
            if (str.Length != 2)
                value = "0";
            else
                value = str[1];
            AddToUndoHistoryList();
            string ExceptionAnswer = _graph.SetValueCell(e.Row.GetIndex(), e.Column.DisplayIndex, value);
            if (ExceptionAnswer != "")
            {
                MessageBox.Show(ExceptionAnswer);
            }
            _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);

        }


        private ObservableCollection<string> _history = new ObservableCollection<string>();
        public ObservableCollection<string> History
        {
            get { return _history; }
            set
            {
                _history = value;
                OnPropertyChanged("History");
            }
        }
        #endregion

        #region work with mouse in canvas and radiogroup

        private ICommand _radioCommand;
        public ICommand RadioCommand
        {
            get
            {
                if (_radioCommand == null)
                    _radioCommand = new RelayCommand((param) => { RadioMethod(param); });

                return _radioCommand;
            }
        }
        private void RadioMethod(object parametr)
        {
            _selectedVertex1 = null;
            _selectedVertex2 = null;

            _canvas.Cursor = Cursors.Cross;
            switch (parametr.ToString())
            {
                case "IsCursor":
                    _selectedAction = SelectedAction.IsCursor;
                    _canvas.Cursor = Cursors.Arrow;



                    break;
                case "IsMove":
                    _selectedAction = SelectedAction.IsMove;


                    break;
                case "IsVertex":
                    _selectedAction = SelectedAction.IsVertex;
                    break;
                case "IsEdge":
                    _selectedAction = SelectedAction.IsEdge;
                    break;
                case "IsDelete":
                    _selectedAction = SelectedAction.IsDelete;
                    break;
                case "IsDeleteAll":
                    if (System.Windows.Forms.MessageBox.Show("Вы уверены, что хотите удалить данный граф?", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        AddToUndoHistoryList();
                        _graph = new Graph();
                        OnPropertyChanged("Matrix");
                        _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
                    }

                    _canvas.Cursor = Cursors.Arrow;
                    break;
            }


        }

        public void MousePressDown(object sender, MouseEventArgs e)
        {
            double x = e.GetPosition(_canvas).X;
            double y = e.GetPosition(_canvas).Y;

            switch (_selectedAction)
            {
                case SelectedAction.IsVertex:
                    string name = InputBox.ShowInputBox("Введите название");
                    if (name != null && name.Length > 0)
                    {
                        AddToUndoHistoryList();
                        if (!_graph.AddVertex(new Vertex(name, new Point(x, y))))
                        {
                            MessageBox.Show("Такая вершина уже есть");
                        }
                        else
                        {
                            OnPropertyChanged("Matrix");

                            _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
                        }
                    }
                    break;
                case SelectedAction.IsEdge:
                    if (_selectedVertex1 == null)
                    {
                        _selectedVertex1 = _graph.FindVertex(new Point(x, y));
                        _graph.DrawSelectedVertex(_canvas, _selectedVertex1, Color.FromArgb(250, 255, 0, 0));
                    }
                    else
                    {
                        _selectedVertex2 = _graph.FindVertex(new Point(x, y));
                        if (_selectedVertex2 != null)
                        {
                            string weidth = InputBox.ShowInputBox("Введите вес");
                            if (weidth != null && weidth.Length > 0)
                            {
                                try
                                {
                                    AddToUndoHistoryList();
                                    if (System.Windows.Forms.MessageBox.Show("Сделать ребро ориентированным?", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                                        _graph.AddEdge(_selectedVertex1, _selectedVertex2, Int32.Parse(weidth), 1);
                                    else
                                        _graph.AddEdge(_selectedVertex1, _selectedVertex2, Int32.Parse(weidth), 0);
                                }
                                catch (FormatException exc)
                                {

                                    if (exc.Source != null)
                                        MessageBox.Show("формат веса ошибочен. Можно вводить толкьо числа");

                                    return;
                                }
                                _selectedVertex1 = null;
                                _selectedVertex2 = null;
                                OnPropertyChanged("Matrix");
                                _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
                            }
                        }
                    }
                    break;
                case SelectedAction.IsMove:
                    if (_selectedVertex1 == null)
                    {
                        _selectedVertex1 = _graph.FindVertex(new Point(x, y));
                        _graph.DrawSelectedVertex(_canvas, _selectedVertex1, Color.FromArgb(250, 255, 0, 0));
                    }
                    else
                    {
                        AddToUndoHistoryList();
                        _graph.MoveVertex(_selectedVertex1, new Point(x, y));
                        _selectedVertex1 = null;
                        _selectedVertex2 = null;
                        _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
                    }
                    break;
                case SelectedAction.IsDelete:
                    var vec = _graph.FindVertex(new Point(x, y));
                    if (vec != null)
                    {
                        _graph.DrawSelectedVertex(_canvas, vec, Color.FromArgb(250, 255, 0, 0));
                        if (System.Windows.Forms.MessageBox.Show("Вы уверены, что хотите удалить данную вершину?", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            AddToUndoHistoryList();
                            _graph.DeleteVertex(vec);
                            OnPropertyChanged("Matrix");
                            _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
                        }
                    }
                    break;
            }



        }

        #endregion

        #region handler close window
        private ICommand _exitCommand;
        public ICommand ExitCommand =>
            _exitCommand ??
            (_exitCommand = new RelayCommand(obj =>
            {
                System.Windows.Application.Current.Shutdown();
            }));
        public void WindowClosing(object sender, CancelEventArgs e)
        {
            //todo
            if (_undoHistoryList.Count > 0)
            {
                if (System.Windows.Forms.MessageBox.Show("У вас есть не сохраненные изменения. Сохранить их?", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (_pathSaveFile != "")
                    {
                        if (System.Windows.Forms.MessageBox.Show("Вы хотите сохранить по существующему пути?\n" + _pathSaveFile, "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                        {
                            var dialog = new SaveFileDialog();
                            dialog.DefaultExt = ".graph";
                            dialog.Filter = "Graph (.graph)|*.graph";
                            if (dialog.ShowDialog() == true)
                            {
                                _pathSaveFile = dialog.FileName;
                            }
                        }
                    }
                    else
                    {
                        var dialog = new SaveFileDialog();
                        dialog.DefaultExt = ".graph";
                        dialog.Filter = "Graph (.graph)|*.graph";
                        if (dialog.ShowDialog() == true)
                        {
                            _pathSaveFile = dialog.FileName;
                        }
                    }
                    if (_pathSaveFile != "") 
                        SaveGraphInFile.SerializableGraphFile(_pathSaveFile, _graph);
                   
                }
            }

        }
        #endregion

        #region work with menu Item
        #region open file,save file

        private Graph DownloadGraphFromFile(Graph graph = null)
        {
            Graph graphTemp = graph;
            if (graphTemp == null)
                graphTemp = new Graph();

            var dialog = new OpenFileDialog();
            dialog.Filter = "Graph Files(*.adj;*.inc;*.edge;*.vertex;*.graph)|*.adj;*.inc;*.edge;*.vertex;*.graph";
            if (dialog.ShowDialog() == true)
            {

                switch (Path.GetExtension(dialog.FileName))
                {
                    case ".graph":
                        Graph gr = OpenGraphFromFile.SerializableGraphFile(dialog.FileName);
                        if (gr==null)
                            MessageBox.Show("Ошибка при октрытие файла графа");
                        else
                        {
                            _pathSaveFile = dialog.FileName;
                            graphTemp = gr;
                         
                        }
    
                        break;
                    case ".adj":
                        AddToUndoHistoryList();
                        if (!graphTemp.OpenAdjacencyMatrixFile(dialog.FileName, (int)_canvas.Width, (int)_canvas.Height))
                        {
                            MessageBox.Show("Произошла ошибка при чтении файла");
                        }
                        break;
                    case ".inc":
                        AddToUndoHistoryList();
                        if (!graphTemp.OpenIncidenceMatrixFile(dialog.FileName, (int)_canvas.Width, (int)_canvas.Height))
                        {
                            MessageBox.Show("Произошла ошибка при чтении файла");
                        }
                        break;
                    case ".edge":
                        AddToUndoHistoryList();
                        if (!graphTemp.OpenEdgeFile(dialog.FileName, (int)_canvas.Width, (int)_canvas.Height))
                        {
                            MessageBox.Show("Произошла ошибка при чтении файла");
                        }
                        break;
                    case ".vertex":
                        AddToUndoHistoryList();
                        if (!graphTemp.OpenVertexFile(dialog.FileName, (int)_canvas.Width, (int)_canvas.Height))
                        {
                            MessageBox.Show("Произошла ошибка при чтении файла");
                        }
                        break;
                }


            }
            return graphTemp;
        }
        private ICommand _newFileCommand;
        public ICommand NewFileCommand =>
            _newFileCommand ??
            (_newFileCommand = new RelayCommand(obj =>
            {
                // диалог создания файла открыть
                _graph = DownloadGraphFromFile(_graph);
                OnPropertyChanged("Matrix");
                _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
            }));
        private ICommand _saveGraphCommand;
        public ICommand SaveGraphCommand =>
            _saveGraphCommand ??
            (_saveGraphCommand = new RelayCommand(obj =>
            {
                if (_pathSaveFile == "")
                {
                    var dialog = new SaveFileDialog();
                    dialog.DefaultExt = ".graph";
                    dialog.Filter = "Graph (.graph)|*.graph";
                    if (dialog.ShowDialog() == true)
                    {
                        _pathSaveFile = dialog.FileName;

                    }

                }
                if (_pathSaveFile == "") return;
                SaveGraphInFile.SerializableGraphFile(_pathSaveFile, _graph);
                _undoHistoryList.Clear();
                _redoHistoryList.Clear();

            }));

        private ICommand _saveAdjacencyCommand;
        public ICommand SaveAdjacencyCommand =>
            _saveAdjacencyCommand ??
            (_saveAdjacencyCommand = new RelayCommand(obj =>
            {

                var dialog = new SaveFileDialog();
                dialog.DefaultExt = ".adj";
                dialog.Filter = "Adjacency (.adj)|*.adj";
                if (dialog.ShowDialog() == true)
                {
                    _graph.SaveAdjacencyMatrixFile(dialog.FileName);
                }
            }));
        private ICommand _saveIncidenceCommand;
        public ICommand SaveIncidenceCommand =>
            _saveIncidenceCommand ??
            (_saveIncidenceCommand = new RelayCommand(obj =>
            {
                var dialog = new SaveFileDialog();
                dialog.DefaultExt = ".inc";
                dialog.Filter = "Incidence (.inc)|*.inc";
                if (dialog.ShowDialog() == true)
                {
                    _graph.SaveIncidenceMatrixFile(dialog.FileName);
                }
            }));
        private ICommand _saveEdgeListCommand;
        public ICommand SaveEdgeListCommand =>
            _saveEdgeListCommand ??
            (_saveEdgeListCommand = new RelayCommand(obj =>
            {
                // диалог создания файла открыть
                var dialog = new SaveFileDialog();
                dialog.DefaultExt = ".edge";
                dialog.Filter = "Edge (.edge)|*.edge";
                if (dialog.ShowDialog() == true)
                {
                    _graph.SaveEdgeFile(dialog.FileName);
                }
            }));
        private ICommand _saveVertexListCommand;
        public ICommand SaveVertexListCommand =>
            _saveVertexListCommand ??
            (_saveVertexListCommand = new RelayCommand(obj =>
            {
                // диалог создания файла открыть
                var dialog = new SaveFileDialog();
                dialog.DefaultExt = ".vertex";
                dialog.Filter = "Vertex (.vertex)|*.vertex";
                if (dialog.ShowDialog() == true)
                {
                    _graph.SaveVertexFile(dialog.FileName);
                }
            }));
        private ICommand _savePictureCommand;
        public ICommand SavePictureCommand =>
            _savePictureCommand ??
            (_savePictureCommand = new RelayCommand(obj =>
            {
                // диалог создания файла открыть
                SaveFileDialog saveimg = new Microsoft.Win32.SaveFileDialog();
                saveimg.DefaultExt = ".png";
                saveimg.Filter = "Image (.png)|*.png";
                if (saveimg.ShowDialog() == true)
                {

                    SaveGraphInFile.PictureGraph(saveimg.FileName, _canvas);                  
                }

            }));
        #endregion

        #region undo and redo event
        private ICommand _undoCommand;//отменитть
        public ICommand UndoCommand =>
            _undoCommand ??
            (_undoCommand = new RelayCommand(obj =>
            {
                if (_undoHistoryList.Count == 0)
                {
                    MessageBox.Show("История пуста");
                    return;
                }
                _redoHistoryList.Add((Graph)_graph.Clone());
                if (_redoHistoryList.Count == 11)
                {
                    _redoHistoryList.RemoveAt(0);
                }
                _graph = _undoHistoryList[_undoHistoryList.Count - 1];
                _undoHistoryList.RemoveAt(_undoHistoryList.Count - 1);
                OnPropertyChanged("Matrix");
                _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
            }));
        private void AddToUndoHistoryList()
        {
            _undoHistoryList.Add((Graph)_graph.Clone());
            if (_undoHistoryList.Count == 11)
            {
                _undoHistoryList.RemoveAt(0);
            }
        }
        private ICommand _redoCommand;//вернуть
        public ICommand RedoCommand =>
            _redoCommand ??
            (_redoCommand = new RelayCommand(obj =>
            {
                if (_redoHistoryList.Count == 0)
                {
                    MessageBox.Show("История пуста");
                    return;
                }
                AddToUndoHistoryList();
                _graph = _redoHistoryList[_redoHistoryList.Count - 1];
                _redoHistoryList.RemoveAt(_redoHistoryList.Count - 1);
                OnPropertyChanged("Matrix");
                _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
            }));
        #endregion

        #region aboutprogram
        private ICommand _aboutProgramCommand;
        public ICommand AboutProgramCommand =>
            _aboutProgramCommand ??
            (_aboutProgramCommand = new RelayCommand(obj =>
            {
                try
                {
                    string str;
                    using (StreamReader sr = new StreamReader(@"D:\6sem\labGraph\labGraph\Resources\AboutProgram.txt"))
                    {
                        str = sr.ReadToEnd();

                    }
                    MessageBox.Show(str);
                }
                catch (Exception)
                {
                    MessageBox.Show("Файл с информацией о программе не найден");
                }
            }));
        private ICommand _aboutAuthorCommand;
        public ICommand AboutAuthorCommand =>
            _aboutAuthorCommand ??
            (_aboutAuthorCommand = new RelayCommand(obj =>
            {
                try
                {
                    string str;
                    using (StreamReader sr = new StreamReader(@"D:\6sem\labGraph\labGraph\Resources\AboutAuthor.txt"))
                    {
                        str = sr.ReadToEnd();

                    }
                    MessageBox.Show(str);
                }
                catch (Exception)
                {
                    MessageBox.Show("Файл с информацией об авторе не найден");
                }
            }));
        #endregion

        #endregion

        #region implementation of the laboratory selection handler
        private void FindRouteInGraph(Func<Vertex, List<List<Vertex>>> foo)
        {
            string names = InputBox.ShowInputBox("Введите название 1 и 2 вершины через пробел");
            string[] str = new string[1];
            if (names != null && names.Length > 0)
                str = names.Split(' ');
            if (str.Length == 2)
            {
                Vertex ver1 = _graph.FindVertex(str[0]);
                Vertex ver2 = _graph.FindVertex(str[1]);
                if (ver1 != null && ver2 != null)
                {
                    _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
                    var routes = foo(ver1);
                    var route = routes[_graph.VertexNames.IndexOf(ver2)];
                    if (route.Count > 0)
                    {
                        _graph.DrawRoute(_canvas, route, Color.FromArgb(250, 255, 0, 0), Brushes.Red);


                    }
                    else
                    {
                        MessageBox.Show("Соединяющего пути не найдено");
                    }


                    List<string> strForPrint = new List<string>();
                    string strRoute = "";
                    foreach (var ver in _graph.VertexNames)
                    {
                        strRoute += ver.ToString() + " ";
                    }
                    strForPrint.Add(strRoute);

                    strForPrint.Add(GraphFunctionality.GetVectorRoutes(routes));

                    PrintInFile("Хотите сохранить данный вектор расстояний?", strForPrint);


                    strForPrint.Clear();

                    foreach (var ver in _graph.VertexNames)
                    {
                        List<List<Vertex>> routesForMatrix;
                        if (ver != ver1)
                            routesForMatrix = foo(ver);
                        else
                            routesForMatrix = routes;
                        strForPrint.Add(GraphFunctionality.GetVectorRoutes(routesForMatrix));

                    }

                    strForPrint.Insert(0, strRoute);


                    PrintInFile("Хотите сохранить матрицу расстояний?", strForPrint);



                }
                else
                {
                    MessageBox.Show("Данные вершины не найдены");
                }
            }
            else
            {
                MessageBox.Show("Ввод не корректен, пожалуйста, введите например:A B");
            }
        }
        private void PrintInFile(string answer, List<string> strForPrint)
        {
            if (System.Windows.Forms.MessageBox.Show(answer, "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                var dialog = new SaveFileDialog();
                dialog.DefaultExt = ".txt";
                dialog.Filter = "Document (.txt)|*.txt";
                if (dialog.ShowDialog() == true)
                {
                    using (StreamWriter sw = new StreamWriter(dialog.FileName, false, System.Text.Encoding.Default))
                    {
                        foreach (var s in strForPrint)
                            sw.WriteLine(s);

                    }
                }

            }
        }

        private ICommand _bFS2Command;
        public ICommand BFS2Command =>
            _bFS2Command ??
            (_bFS2Command = new RelayCommand(obj =>
            {
                FindRouteInGraph(_graph.BreadthFirstSearch);

            }));
        private ICommand _bFS3Command;
        public ICommand BFS3Command =>
            _bFS3Command ??
            (_bFS3Command = new RelayCommand(obj =>
            {
                FindRouteInGraph(_graph.BestFirstSearch);
            }));
        private ICommand _dijkstraAlgorithmCommand;
        public ICommand DijkstraAlgorithmCommand =>
            _dijkstraAlgorithmCommand ??
            (_dijkstraAlgorithmCommand = new RelayCommand(obj =>
            {
                FindRouteInGraph(_graph.DijkstraAlgorithm);
            }));

        private ICommand _aStarAlgorithmCommand;
        public ICommand AStarAlgorithmCommand =>
            _aStarAlgorithmCommand ??
            (_aStarAlgorithmCommand = new RelayCommand(obj =>
            {
            string names = InputBox.ShowInputBox("Введите название 1 и 2 вершины через пробел");
            string[] str = new string[1];
            if (names != null && names.Length > 0)
                str = names.Split(' ');
            if (str.Length == 2)
            {
                Vertex ver1 = _graph.FindVertex(str[0]);
                Vertex ver2 = _graph.FindVertex(str[1]);
                if (ver1 != null && ver2 != null)
                {
                    _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
                    var route = _graph.AStarAlgorithm(ver1, ver2);

                    if (route.Count > 0)
                    {
                        _graph.DrawRoute(_canvas, route, Color.FromArgb(250, 255, 0, 0),Brushes.Red);


                    }
                    else
                    {
                        MessageBox.Show("Соединяющего пути не найдено");
                    }


                    List<string> strForPrint = new List<string>();
                    string nameVertex = "";
                    string strRoute = "";
                        foreach (var ver in _graph.VertexNames)
                        {
                            var routeTemp = _graph.AStarAlgorithm(ver1, ver);
                            if (routeTemp.Count > 0)
                            {
                                strRoute += routeTemp[routeTemp.Count - 1].Weight + " ";
                            }
                            else
                                strRoute += "- ";
                            nameVertex += ver.ToString() + " ";
                        }
                        strForPrint.Add(nameVertex);
                        strForPrint.Add(strRoute);

                     

                        PrintInFile("Хотите сохранить данный вектор расстояний?", strForPrint);


                        strForPrint.Clear();
                        strForPrint.Add(nameVertex);
                        foreach (var vertex1 in _graph.VertexNames)
                        {
                            var routeStr = "";
                            foreach (var vertex2 in _graph.VertexNames)
                            {
                                var routeTemp = _graph.AStarAlgorithm(vertex1, vertex2);
                                if (routeTemp.Count > 0)
                                {
                                    routeStr += routeTemp[routeTemp.Count - 1].Weight + " ";
                                }
                                else
                                    routeStr += "- ";


                            }
                            strForPrint.Add(routeStr);
                        }




                        PrintInFile("Хотите сохранить матрицу расстояний?", strForPrint);

    

                    }
                    else
                    {
                        MessageBox.Show("Данные вершины не найдены");
                    }
                }
                else
                {
                    MessageBox.Show("Ввод не корректен, пожалуйста, введите например:A B");
                }
            }));
        

        private ICommand _getWeightsVertexCommand;
        public ICommand GetWeightsVertexCommand =>
             _getWeightsVertexCommand ??
            (_getWeightsVertexCommand = new RelayCommand(obj =>
            {
                var list = _graph.GetVertexWeights();
                string str = "";
                foreach (var ver in list)
                {
                    str += ver.ToString() + "(" + ver.Weight + ") ";
                }
                History.Add("Веса вершин графа:");
                History.Add(str);
                PrintInFile("Хотите сохранить веса вершин графа?", new List<string> { "Веса вершин:", str });
            }));
        private ICommand _getRadiusCommand;
        public ICommand GetRadiusCommand =>
             _getRadiusCommand ??
            (_getRadiusCommand = new RelayCommand(obj =>
            {
                History.Add("Радиус графа:");
                string str = _graph.SearchRadius().ToString();
                History.Add(str);
                PrintInFile("Хотите сохранить радиус графа?", new List<string> { "Радиус:", str });
            }));
        private ICommand _getDiameterCommand;
        public ICommand GetDiameterCommand =>
             _getDiameterCommand ??
            (_getDiameterCommand = new RelayCommand(obj =>
            {
                History.Add("Диаметр графа:");
                string str = _graph.SearchDiameter().ToString();
                History.Add(str);
                PrintInFile("Хотите сохранить диаметр графа?", new List<string> { "Диаметр:", str });
            }));
        private ICommand _getListDegreeVertexCommand;
        public ICommand GetListDegreeVertexCommand =>
             _getListDegreeVertexCommand ??
            (_getListDegreeVertexCommand = new RelayCommand(obj =>
            {
                var list = _graph.GetDegreeVertexGrapf();
                string str = "";
                foreach (var ver in list)
                {
                    str += "deg(" + ver.ToString() + ")=" + ver.Deegre + "   ";
                }
                History.Add("Степени вершин графа:");
                History.Add(str);
                PrintInFile("Хотите сохранить степени вершин графа?", new List<string> { str });
            }));

        private ICommand _isomorphismCommand;
        public ICommand IsomorphismCommand =>
             _isomorphismCommand ??
            (_isomorphismCommand = new RelayCommand(obj =>
            {
                Graph gr = DownloadGraphFromFile();
                if (gr == null)
                {
                    MessageBox.Show("Ошибка при открытии файла");
                    return;
                }
                if (_graph.IsIsomorphism(gr))
                {
                    History.Add("Изоморфны");
                }
                else
                    History.Add("Не изоморфны");


            }));

        private ICommand _isFullGraphCommand;
        public ICommand IsFullGraphCommand =>
             _isFullGraphCommand ??
            (_isFullGraphCommand = new RelayCommand(obj =>
            {
                if (_graph.IsFullGraph())
                    History.Add("Граф полный");
                else
                    History.Add("Граф не полный");



            }));
        private ICommand _getAdditionGraphCommand;
        public ICommand GetAdditionGraphCommand =>
             _getAdditionGraphCommand ??
            (_getAdditionGraphCommand = new RelayCommand(obj =>
            {
                AddToUndoHistoryList();
                _graph = _graph.GetAdditionGraph();
                OnPropertyChanged("Matrix");
                _graph.Draw(_canvas, _selectedVertex1, _selectedVertex2);
                if (System.Windows.Forms.MessageBox.Show("Хотите сохранить данный граф?", "", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {

                    var dialog = new SaveFileDialog();
                    dialog.DefaultExt = ".graph";
                    dialog.Filter = "Graph (.graph)|*.graph";
                    if (dialog.ShowDialog() == true)
                    {
                        SaveGraphInFile.SerializableGraphFile(dialog.FileName, _graph);
                       
                    }




                }
            }));
        private ICommand _findChromaticNumber;
        public ICommand FindChromaticNumber =>
             _findChromaticNumber ??
            (_findChromaticNumber = new RelayCommand(obj =>
            {
                History.Add("Хроматическое число равно: "+_graph.FindChromaticNumber(_canvas).ToString());
            }));
        
        #endregion



    }
}
