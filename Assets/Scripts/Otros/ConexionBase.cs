using UnityEngine;
using System.Net;
using System.Net.Mail;
using System;

public class ConexionBase : MonoBehaviour
{
    private string usuario = "tucorreogmail@gmail.com";
    private string contraseña = "tucontraseñagmail";
    private string correoDestino = "correodestino@gmail.com";
    private string codigoVerificacion;

    private void Start()
    {
        // Generar un código de verificación único
        codigoVerificacion = GenerarCodigoVerificacion(6);
        // Enviar correo de verificación
        EnviarCorreoVerificacion();
    }

    private void EnviarCorreoVerificacion()
    {
        MailMessage correo = new MailMessage(usuario, correoDestino)
        {
            Subject = "Código de Verificación Jakay",
            Body = "Tu código de verificación es: " + codigoVerificacion
        };

        SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(usuario, contraseña),
            EnableSsl = true
        };

        try
        {
            smtpClient.Send(correo);
            Debug.Log("Correo de verificación enviado con éxito.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al enviar el correo de verificación: " + ex.Message);
        }
    }
    private string GenerarCodigoVerificacion(int longitud)
    {
        const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] codigo = new char[longitud];
        System.Random random = new System.Random();

        for (int i = 0; i < longitud; i++)
        {
            codigo[i] = caracteres[random.Next(caracteres.Length)];
        }

        return new string(codigo);
    }
}
