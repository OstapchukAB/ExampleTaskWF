namespace AsyncTaskExample;

public partial class ProgressForm : Form
{
    private readonly CancellationTokenSource? _cancellationTokenSource;

    public ProgressForm(CancellationTokenSource cancellationTokenSource)
    {
        InitializeComponent();
        _cancellationTokenSource = cancellationTokenSource;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel(); // Прерывание задачи
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cancellationTokenSource?.Cancel(); // Прерывание при закрытии формы
        base.OnFormClosing(e);
    }

    public void UpdateProgress(int value) // обновления прогресса
    {
        if (progressBar.InvokeRequired)
        {
            progressBar.Invoke(new Action(() => 
            {
                progressBar.Value = value;
                lblProgressPercentage.Text = $"{value}%"; // Обновление текста с процентами
            }));
        }
        else
        {
            progressBar.Value = value;
            lblProgressPercentage.Text = $"{value}%";
        }
    }
}
partial class ProgressForm
{
    private System.ComponentModel.IContainer components = null;
    private Button btnCancel;
    private ProgressBar progressBar; // Новая переменная для ProgressBar
    private Label lblProgressPercentage; // Новый элемент для процентов


    private void InitializeComponent()
    {
        this.btnCancel = new System.Windows.Forms.Button();
        this.progressBar = new System.Windows.Forms.ProgressBar();
        this.lblProgressPercentage = new System.Windows.Forms.Label(); 
       this.SuspendLayout();

        // 
        // btnCancel
        // 
        this.btnCancel.Location = new System.Drawing.Point(12, 70);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(150, 30);
        this.btnCancel.TabIndex = 0;
        this.btnCancel.Text = "Прервать задачу";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

        // 
        // progressBar
        // 
        this.progressBar.Location = new System.Drawing.Point(12, 12); // Новая строка: размещение ProgressBar
        this.progressBar.Name = "progressBar";
        this.progressBar.Size = new System.Drawing.Size(260, 30);
        this.progressBar.TabIndex = 1;


        // 
        // lblProgressPercentage
        // 
        this.lblProgressPercentage.Location = new System.Drawing.Point(12, 45); // Добавлено над кнопкой
        this.lblProgressPercentage.Name = "lblProgressPercentage";
        this.lblProgressPercentage.Size = new System.Drawing.Size(260, 20);
        this.lblProgressPercentage.TabIndex = 2;
        this.lblProgressPercentage.Text = "0%"; // Изначальное значение процентов


        // 
        // ProgressForm
        // 
        this.ClientSize = new System.Drawing.Size(284, 111);
        this.Controls.Add(this.lblProgressPercentage); // Добавление процентов
        this.Controls.Add(this.progressBar); // Новая строка: добавление ProgressBar
        this.Controls.Add(this.btnCancel);
        this.Name = "ProgressForm";
        this.Text = "Прогресс";
        this.ResumeLayout(false);
    }
}

