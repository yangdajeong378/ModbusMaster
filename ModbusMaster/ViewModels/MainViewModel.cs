using ModbusMaster.Views;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;


namespace ModbusMaster.ViewModels
{
    public enum Registers
    {
        OutputCoils,
        DiscreteInputs,
        InputRegisters,
        HoldingRegisters
    }

    public class MainViewModel  : INotifyPropertyChanged
    {
        public List<Registers> RegistersList { get; }


        private Registers _selectedRegister;
        public Registers SelectedRegister
        {
            get { return _selectedRegister; }
            set
            {
                _selectedRegister = value;
                OnPropertyChanged(nameof(SelectedRegister));       
            }
        }


        public ICommand ConnectCommand { get; }
        public ICommand ReadWriteDefinitionCommad { get; }
        public ICommand WarningCommand { get; }

        public MainViewModel()
        {
            //Enum.GetValues는 기본적으로 Array 타입으로 값을 반환함 그걸 Registers[]으로 반환한다
            //Enum.GetValues(typeof(Registers)) : 지정된 enum의 모든 값을 배열로 반환함

            //enum을 리스트에 넣어주는 작업
            RegistersList = new List<Registers>((Registers[])Enum.GetValues(typeof(Registers)));

            ConnectCommand = new RelayCommand(OpenConnect);
            ReadWriteDefinitionCommad = new RelayCommand(OpenSetup);
            WarningCommand = new RelayCommand(OpenWarning);
        }

        private void OpenConnect() 
        {
            var connetView = new ConnectView();
            bool? result = connetView.ShowDialog();
        }

        private void OpenSetup()
        {
            var setupView = new SetupView();
            bool? result = setupView.ShowDialog();
        }

        private void OpenWarning()
        {
            var warning = new WarningView();
            bool? result = warning.ShowDialog();

            // 결과 처리
            if (result == true)
            {
                MessageBox.Show("연결이 끊겼습니다.");
            }
            //else if (result == false)
            //{
            //    MessageBox.Show("Dialog closed with Cancel.");
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
