using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerUserConnection
{
    public class VoiceMessage
    {
        public bool IsSound = true;
        public bool IsSendVoice = true;
        //поток для нашей речи
        WaveIn input;
        //поток для речи собеседника
        WaveOut output;
        //буфферный поток для передачи через сеть
        BufferedWaveProvider bufferStream;
        //поток для прослушивания входящих сообщений
        Thread in_thread;
        ClientServerUdp udpAudio;
        public VoiceMessage(int portRecive, int portRemote, string iPAdressRemote)
        {
            udpAudio = new ClientServerUdp(portRecive, portRemote, iPAdressRemote);
        }


        public void StartVoiceMessage()
        {
            //создаем поток для записи нашей речи
            input = new WaveIn
            {
                //определяем его формат - частота дискретизации 8000 Гц, ширина сэмпла - 16 бит, 1 канал - моно
                WaveFormat = new WaveFormat(8000, 16, 1)
            };
            //добавляем код обработки нашего голоса, поступающего на микрофон
            input.DataAvailable += Voice_Input;
            //создаем поток для прослушивания входящего звука
            output = new WaveOut();
            //создаем поток для буферного потока и определяем у него такой же формат как и потока с микрофона
            bufferStream = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            //привязываем поток входящего звука к буферному потоку
            output.Init(bufferStream);
            udpAudio.IcomingMessanger += Listening;
            input.StartRecording();
            in_thread = new Thread(new ThreadStart(output.Play));
            in_thread.IsBackground = true;
            udpAudio.StartListening();
            //запускаем его
            in_thread.Start();
        }
        public void StopVoiceMessage()
        {
            if (output != null)
            {
                output.Stop();
                output.Dispose();
                output = null;
            }
            if (input != null)
            {
                input.Dispose();
                input = null;
            }
            bufferStream = null;
            udpAudio?.Shutdown();
        }
        private void Voice_Input(object sender, WaveInEventArgs e)
        {
            try
            {
                if (IsSendVoice)
                    udpAudio?.SendMessage(e.Buffer);
            }
            catch
            { }
        }

        private void Listening(byte[] b)
        {
            try
            {
                if (IsSound)
                    bufferStream?.AddSamples(b, 0, b.Length);
            }
            catch { }
        }
    }
}
