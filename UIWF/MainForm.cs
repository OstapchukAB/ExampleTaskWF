using Timer = System.Windows.Forms.Timer;

namespace AsyncTaskExample;

public partial class MainForm : Form
{
    private CancellationTokenSource? _cancellationTokenSource;
    private Timer _clockTimer; // Новый таймер для часов

    public MainForm()
    {
        InitializeComponent();
        this.Load += MainForm_Load; // Добавлено: подписка на событие загрузки формы

        // Настройка таймера для часов
        _clockTimer = new Timer();
        _clockTimer.Interval = 1000; // Обновление каждую секунду
        _clockTimer.Tick += (s, e) => lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
        _clockTimer.Start();

        UpdateTaskStatus("Нет активной задачи"); // Изначальный статус
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }

    private async void MainForm_Load(object sender, EventArgs e) // Новый метод
    {
        if (_cancellationTokenSource != null)
        {
            MessageBox.Show("Задача уже выполняется!");
            return;
        }

        // Создаем токен отмены
        _cancellationTokenSource = new CancellationTokenSource();
        UpdateTaskStatus("Задача выполняется..."); // Обновление статуса

        // Создаем и показываем форму прогресса
        using (var progressForm = new ProgressForm(_cancellationTokenSource))
        {
            progressForm.Show();

            try
            {
                // Выполнение асинхронной задачи
                await RunLongTaskAsync(_cancellationTokenSource.Token, progressForm);
                UpdateTaskStatus("Задача завершена."); // Обновление статуса
            }
            catch (OperationCanceledException)
            {
                UpdateTaskStatus("Задача прервана."); // Обновление статуса
                MessageBox.Show("Задача была прервана.");
            }
            finally
            {
                _cancellationTokenSource = null;
            }
        }
    }

    private async void btnStartTask_Click(object sender, EventArgs e)
    {
        if (_cancellationTokenSource != null)
        {
            MessageBox.Show("Задача уже выполняется!");
            return;
        }

        // Создаем токен отмены
        _cancellationTokenSource = new CancellationTokenSource();
        UpdateTaskStatus("Задача выполняется..."); // Обновление статуса

        // Создаем и показываем форму прогресса
        using (var progressForm = new ProgressForm(_cancellationTokenSource))
        {
            progressForm.Show();

            try
            {
                // Выполнение асинхронной задачи
                await RunLongTaskAsync(_cancellationTokenSource.Token,progressForm);
                UpdateTaskStatus("Задача завершена."); // Обновление статуса
            }
            catch (OperationCanceledException)
            {
                UpdateTaskStatus("Задача прервана."); // Обновление статуса
                MessageBox.Show("Задача была прервана.");
            }
            finally
            {
                _cancellationTokenSource = null;
            }
        }
    }

    private async Task RunLongTaskAsync(CancellationToken token,ProgressForm progressForm)
    {
        for (int i = 1; i <= 100; i++)
        {
            token.ThrowIfCancellationRequested();
            progressForm.UpdateProgress(i); //  обновление прогресса
            await Task.Delay(100, token); // Имитация ввода/вывода
        }
    }

    private void btnCancelTask_Click(object sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel();
    }

    private void UpdateTaskStatus(string status) // Новый метод
    {
        if (lblTaskStatus.InvokeRequired)
        {
            lblTaskStatus.Invoke(new Action(() => lblTaskStatus.Text = status));
        }
        else
        {
            lblTaskStatus.Text = status;
        }
    }
}
partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private Button btnStartTask;
    private Button btnCancelTask;
    private Label lblClock; //  часы
    private Label lblTaskStatus; //  статус задачи

    private void InitializeComponent()
    {
        this.btnStartTask = new System.Windows.Forms.Button();
        this.btnCancelTask = new System.Windows.Forms.Button();
        this.lblClock = new System.Windows.Forms.Label(); // Инициализация часов
        this.lblTaskStatus = new System.Windows.Forms.Label();
        this.SuspendLayout();

        // 
        // btnStartTask
        // 
        this.btnStartTask.Location = new System.Drawing.Point(12, 12);
        this.btnStartTask.Name = "btnStartTask";
        this.btnStartTask.Size = new System.Drawing.Size(150, 30);
        this.btnStartTask.TabIndex = 0;
        this.btnStartTask.Text = "Запустить задачу";
        this.btnStartTask.UseVisualStyleBackColor = true;
        this.btnStartTask.Click += new System.EventHandler(this.btnStartTask_Click);

        // 
        // btnCancelTask
        // 
        this.btnCancelTask.Location = new System.Drawing.Point(12, 50);
        this.btnCancelTask.Name = "btnCancelTask";
        this.btnCancelTask.Size = new System.Drawing.Size(150, 30);
        this.btnCancelTask.TabIndex = 1;
        this.btnCancelTask.Text = "Отменить задачу";
        this.btnCancelTask.UseVisualStyleBackColor = true;
        this.btnCancelTask.Click += new System.EventHandler(this.btnCancelTask_Click);


        // lblClock
        // 
        this.lblClock.Location = new System.Drawing.Point(12, 90);
        this.lblClock.Name = "lblClock";
        this.lblClock.Size = new System.Drawing.Size(260, 20);
        this.lblClock.TabIndex = 2;
        this.lblClock.Text = "00:00:00"; // Изначальное значение часов

        // 
        // lblTaskStatus
        // 
        this.lblTaskStatus.Location = new System.Drawing.Point(12, 120); // Новый элемент ниже часов
        this.lblTaskStatus.Name = "lblTaskStatus";
        this.lblTaskStatus.Size = new System.Drawing.Size(260, 20);
        this.lblTaskStatus.TabIndex = 3;
        this.lblTaskStatus.Text = "Нет активной задачи";

        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(284, 161);
        this.Controls.Add(this.lblTaskStatus); // Добавление статуса задачи на форму
        this.Controls.Add(this.lblClock); // Добавление часов на форму
        this.Controls.Add(this.btnCancelTask);
        this.Controls.Add(this.btnStartTask);
        this.Name = "MainForm";
        this.Text = "Главная форма";
        this.ResumeLayout(false);
    }
}

