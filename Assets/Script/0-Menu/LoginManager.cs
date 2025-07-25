using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelLogin;
    public GameObject panelRegister;

    [Header("Login")]
    public TMP_InputField loginUsername; 
    public TMP_InputField loginPassword;
    public TMP_Text loginMessage;

    [Header("Register")]
    public TMP_InputField registerUsername; 
    public TMP_InputField registerPassword; 
    public TMP_InputField registerEmail; 
    public TMP_InputField registerPhone;
    public TMP_Text registerMessage;


    private string filePath;

    void Start()
    {
        filePath = Application.dataPath + "/Resources/account.txt";
        ShowLoginPanel();
    }

    public void ShowLoginPanel()
    {
        panelLogin.SetActive(true);
        panelRegister.SetActive(false);

        // Xóa nội dung input và thông báo lỗi
        loginUsername.text = "";
        loginPassword.text = "";
        loginMessage.text = "";
    }


    public void ShowRegisterPanel()
    {
        panelLogin.SetActive(false);
        panelRegister.SetActive(true);

        // Xóa nội dung input và thông báo lỗi
        registerUsername.text = "";
        registerPassword.text = "";
        registerEmail.text = "";
        registerPhone.text = "";
        registerMessage.text = "";
    }


    public void Register()
    {
        string username = registerUsername.text.Trim();
        string password = registerPassword.text.Trim();
        string email = registerEmail.text.Trim();
        string phone = registerPhone.text.Trim();

        if (!Regex.IsMatch(username, "^[a-z0-9]{1,20}$"))
        {
            registerMessage.text = "Username chỉ chứa chữ thường và số (tối đa 20 ký tự)";
            return;
        }

        if (!Regex.IsMatch(password, "^(?=.*[@#$%])[a-zA-Z0-9@#$%]{6,20}$"))
        {
            registerMessage.text = "Password phải có ít nhất 6 ký tự, tối đa 20 và chứa @#$%";
            return;
        }

        if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            registerMessage.text = "Email không hợp lệ!";
            return;
        }

        if (!Regex.IsMatch(phone, @"^(03|05|07|08|09)[0-9]{8}$"))
        {
            registerMessage.text = "Số điện thoại không hợp lệ!";
            return;
        }

        if (IsUsernameExists(username))
        {
            registerMessage.text = "Username đã tồn tại!";
            return;
        }

        SaveAccount(username, password, email, phone);
        registerMessage.text = "Đăng ký thành công!";
        ShowLoginPanel();
    }

    private bool IsUsernameExists(string username)
    {
        if (File.Exists(filePath))
        {
            string[] accounts = File.ReadAllLines(filePath);
            foreach (string acc in accounts)
            {
                string[] data = acc.Split('\t');
                if (data[0] == username) return true;
            }
        }
        return false;
    }

    private void SaveAccount(string username, string password, string email, string phone)
    {
        string accountData = $"{username}\t{password}\t{email}\t{phone}";
        File.AppendAllText(filePath, accountData + "\n");
    }

    public void Login()
    {
        string username = loginUsername.text.Trim();
        string password = loginPassword.text.Trim();

        if (File.Exists(filePath))
        {
            string[] accounts = File.ReadAllLines(filePath);
            foreach (string acc in accounts)
            {
                string[] data = acc.Split('\t');
                if (data.Length >= 4 && data[0] == username && data[1] == password)
                {
                    Debug.Log("Đăng nhập thành công!");

                    

                    // Lưu username để dùng trong game
                    PlayerPrefs.SetString("LoggedInUser", data[0]);
                    PlayerPrefs.Save();

                    // Chuyển sang màn hình thông tin
                    panelLogin.SetActive(false);
                    SceneManager.LoadScene("Home");
                    return;
                }
            }
        }
        loginMessage.text = "Sai username hoặc password!";
    }

    // Xử lý Logout
    public void Logout()
    {
        PlayerPrefs.DeleteKey("LoggedInUser"); // Xóa dữ liệu đăng nhập
        ShowLoginPanel();
    }
}