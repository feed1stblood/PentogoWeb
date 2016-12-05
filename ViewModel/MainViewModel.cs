using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using System;
using PentagoWeb.Model.Board;
using System.Windows.Controls;
using System.Windows;
using PentagoWeb.Model;
using System.ComponentModel;
using PentagoWeb.Model.AI;

namespace PentagoWeb.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm/getstarted
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public string Welcome
        {
            get
            {
                return "Welcome to MVVM Light";
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"

            }
            RotateButtonVisibility = Visibility.Collapsed;

            WholeBoard = new PentagoBoard();
            //WholeBoard[0, 0] = new Status(Status.StateEnum.black);
            //WholeBoard[1, 3] = new Status(Status.StateEnum.black);
            //WholeBoard[4, 5] = new Status(Status.StateEnum.black);
            //WholeBoard[5, 2] = new Status(Status.StateEnum.white);
            //WholeBoard[3, 0] = new Status(Status.StateEnum.white);



            //control = new GameControl(this.WholeBoard, null ,new AIPlayer(WholeBoard,Status.StateEnum.black));
            //control = new GameControl(this.WholeBoard, null, null);

            CmdStartHumanVsHuman = new RelayCommand(HumanVsHuman);
            CmdStartComputerVsHuman = new RelayCommand(ComputerVsHuman);
            CmdStartComputerVsComputer = new RelayCommand(ComputerVsComputer);
            CmdStartChallenge = new RelayCommand(StartChallenge);
            CmdStartTestLevel = new RelayCommand(this.StartTestLevels);
            CmdNextGame = new RelayCommand(NextGame);
            CmdVsRandom = new RelayCommand(HumanVsRandomPlayer);

            HumanVsHuman();
        }

        void RotateAction(bool on)
        {
            RotateButtonVisibility = (on) ? Visibility.Visible : Visibility.Collapsed;
        }

        public GameControl Control { get; protected set; }

        void GameOver(Status winner)
        {
            switch (winner.State)
            {
                case Status.StateEnum.empty:
                    GameMessage = "Draw";
                    break;
                case Status.StateEnum.black:
                    GameMessage = "Circle Win";
                    break;
                case Status.StateEnum.white:
                    GameMessage = "Cross Win";
                    break;
            }
        }

        string title;
        public String Title
        { get { return title; } protected set { title = value; RaisePropertyChanged("Title"); } }


        public string[] LevelTestResult { get; protected set; }
        void GameOverTestLevelMode(Status winner)
        {
            string result;
            if (winner.State == Status.StateEnum.empty)
            {
                result = "Draw";
            }
                //player won
            else if((winner.State == Status.StateEnum.black && IsPlayerBlack) || ((winner.State == Status.StateEnum.white && !IsPlayerBlack)))
            {
                result = "Win";
            }
            else
            {
                result = "Lose";
            }
            LevelTestResult[Difficulty / 3] = result;
            RaisePropertyChanged("LevelTestResult");
            if (Difficulty == 9)
            {
                SurveyLink2Visibility = Visibility.Visible;
                Title = "Feedback please.";
            }
            else
            {
                NextGameButtonVisibility = Visibility.Visible;
                Difficulty += 3;
            }
        }

        void GameOverChallengeMode(Status winner)
        {
            if (winner.State == Status.StateEnum.empty)
            {
                PlayerDraw();
            }
                //player won
            else if((winner.State == Status.StateEnum.black && IsPlayerBlack) || ((winner.State == Status.StateEnum.white && !IsPlayerBlack)))
            {
                PlayerWon();
            }
            else
            {
                PlayerLose();
            }

            NextGameButtonVisibility = Visibility.Visible;

            if (GamePlayed >= 5)
            {
                SurveyLinkVisibility = Visibility.Visible;
                Title = "Feedback please.";
            }
        }
       
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}

        Status boardContentSelectedItem = null;

        public Status BoardContentSelectedItem
        {
            get { return boardContentSelectedItem; }
            set 
            {
                
                if (value != null)
                {
                    Control.Placeable = false;
                    RaisePropertyChanged("BoardContentSelectedItem");
                    Control.HumanPlace(value);
                }
            }
        }




        Visibility rotateButtonVisibility;
        public Visibility RotateButtonVisibility
        {
            get { return rotateButtonVisibility; }
            private set
            {
                rotateButtonVisibility = value;
                base.RaisePropertyChanged("RotateButtonVisibility");                
            }
        }

        string gameMessage;

        public string GameMessage
        {
            get { return gameMessage; }
            private set 
            { 
                gameMessage = value;
                base.RaisePropertyChanged("GameMessage"); 
            }
        }
        public PentagoBoard WholeBoard { get; private set; }

        public void HideRotateButton(object sender,EventArgs e)
        {
            RotateButtonVisibility =  Visibility.Collapsed;
            
        }

        void HumanVsHuman()
        {
            WholeBoard = new PentagoBoard();

            //WholeBoard[1, 1].State = Status.StateEnum.black;
            //WholeBoard[1, 2].State = Status.StateEnum.black;
            //WholeBoard[1, 4].State = Status.StateEnum.black;

            //WholeBoard[4, 1].State = Status.StateEnum.white;
            //WholeBoard[4, 3].State = Status.StateEnum.white;
            //WholeBoard[4, 4].State = Status.StateEnum.white;

            base.RaisePropertyChanged("WholeBoard");
            //Control = new GameControl(this.WholeBoard, null, null);
            Control = new GameControl(this.WholeBoard, null, new AIPlayerCut(WholeBoard,1));
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }

        void ComputerVsHuman()
        {
            WholeBoard = new PentagoBoard();
            base.RaisePropertyChanged("WholeBoard");
            Control = new GameControl(this.WholeBoard, null, AIFactory.NextAI(WholeBoard, 2, false, 1));
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }

        void HumanVsRandomPlayer()
        {
            WholeBoard = new PentagoBoard();
            base.RaisePropertyChanged("WholeBoard");
            Control = new GameControl(this.WholeBoard, null, new RandomAIPlayer(WholeBoard));
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }



        void ComputerVsComputer()
        {
            WholeBoard = new PentagoBoard();
            //WholeBoard[1, 2].State = Status.StateEnum.black;
            //WholeBoard[1, 1].State = Status.StateEnum.black;
            //WholeBoard[1, 4].State = Status.StateEnum.black;
            //WholeBoard[4, 1].State = Status.StateEnum.white;
            //WholeBoard[4, 4].State = Status.StateEnum.white;
            base.RaisePropertyChanged("WholeBoard");
            Control = new GameControl(this.WholeBoard, AIFactory.NextAI(WholeBoard, 2, false, 0), new AIPlayer(WholeBoard, 2));
            Control.GameOver = new GameControl.GameOverHandler(GameOver);
            Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
            Control.Start();
        }

        void StartChallenge()
        {
            MenuButtonVisibility = Visibility.Collapsed;
            Difficulty = 1;
            RaisePropertyChanged("Difficulty");
            postGameProcess = new GameControl.GameOverHandler(this.GameOverChallengeMode);
            this.ChaModeVisibility = Visibility.Visible;
            NextGame();
            
            
        }

        void StartTestLevels()
        {
            MenuButtonVisibility = Visibility.Collapsed;
            Difficulty = 0;
            postGameProcess = new GameControl.GameOverHandler(this.GameOverTestLevelMode);
            this.TstModeVisibility = Visibility.Visible;
            LevelTestResult = Enumerable.Repeat("To be determined", 4).ToArray();
            RaisePropertyChanged("LevelTestResult");
            NextGame();
        }

        GameControl.GameOverHandler postGameProcess;

        void NextGame()
        {
            NextGameButtonVisibility = Visibility.Collapsed;
            GameMessage = "";
            Title = "Diff " + Difficulty;
            WholeBoard = new PentagoBoard();
            base.RaisePropertyChanged("WholeBoard");
            var AI = AIFactory.NextAI(WholeBoard, Difficulty / 2);
            if (Difficulty % 2 == 1)
            {
                Control = new GameControl(WholeBoard, AI, null);
                Control.GameOver = postGameProcess;
                Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
                Control.Start();
            }
            else
            {
                Title += " - You go first";
                Control = new GameControl(WholeBoard, null, AI);
                Control.GameOver = postGameProcess;
                Control.RotatabilityChange = new GameControl.RotatabilityChangeHandler(RotateAction);
                Control.Start();
            }
        }



        public RelayCommand CmdStartHumanVsHuman { get; set; }
        public RelayCommand CmdStartComputerVsHuman { get; set; }
        public RelayCommand CmdStartComputerVsComputer { get; set; }
        public RelayCommand CmdStartChallenge { get; set; }
        public RelayCommand CmdNextGame { get; set; }
        public RelayCommand CmdVsRandom { get; set; }
        public RelayCommand CmdStartTestLevel { get; set; }

        Visibility nextGameButtonVisibility = Visibility.Collapsed;
        public Visibility NextGameButtonVisibility
        {
            get { return nextGameButtonVisibility; }
            private set
            {
                nextGameButtonVisibility = value;
                base.RaisePropertyChanged("NextGameButtonVisibility");
            }
        }

        Visibility menuButtonVisibility = Visibility.Visible;
        public Visibility MenuButtonVisibility
        {
            get { return menuButtonVisibility; }
            private set
            {
                menuButtonVisibility = value;
                base.RaisePropertyChanged("MenuButtonVisibility");
            }
        }

        Visibility surveyLinkVisibility = Visibility.Collapsed;
        public Visibility SurveyLinkVisibility
        {
            get { return surveyLinkVisibility; }
            private set
            {
                surveyLinkVisibility = value;
                base.RaisePropertyChanged("SurveyLinkVisibility");
            }
        }

        Visibility surveyLink2Visibility = Visibility.Collapsed;
        public Visibility SurveyLink2Visibility
        {
            get { return surveyLink2Visibility; }
            private set
            {
                surveyLink2Visibility = value;
                base.RaisePropertyChanged("SurveyLink2Visibility");
            }
        }

        Visibility chaModeVisibility = Visibility.Collapsed;
        public Visibility ChaModeVisibility
        {
            get { return chaModeVisibility; }
            private set
            {
                chaModeVisibility = value;
                base.RaisePropertyChanged("ChaModeVisibility");
            }
        }

        Visibility tstModeVisibility = Visibility.Collapsed;
        public Visibility TstModeVisibility
        {
            get { return tstModeVisibility; }
            private set
            {
                tstModeVisibility = value;
                base.RaisePropertyChanged("TstModeVisibility");
            }
        }

        public int Win { get; set; }
        public int Draw { get; set; }
        public int Lose { get; set; }
        public int Difficulty { get; set; }

        int GamePlayed { get { return Win + Draw + Lose; } }

        bool IsPlayerBlack { get { return Difficulty % 2 == 1; } }

        void PlayerWon()
        {
            GameMessage = "You win";
            Difficulty = Math.Min(Difficulty+2,9);
            RaisePropertyChanged("Difficulty");
            Win++;
            RaisePropertyChanged("Win");
        }

        void PlayerDraw()
        {
            GameMessage = "Draw";
            Difficulty = Math.Min(Difficulty + new Random().Next(2), 9);
            RaisePropertyChanged("Difficulty");
            Draw++;
            RaisePropertyChanged("Draw");
        }

        void PlayerLose()
        {
            GameMessage = "You lose";
            Difficulty = Math.Max(0, Difficulty - 1);
            RaisePropertyChanged("Difficulty");
            Lose++;
            RaisePropertyChanged("Lose");
        }
    }
}