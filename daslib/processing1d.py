
from scipy.signal import butter, filtfilt, lfilter
#funciones para procesar las series temporales
class Processing1D:
    def __init__(self, signal):
        self.signal = signal
        pass

    def get_signal(self):
        return self.signal
    
    #static method to bandpass filter
    @staticmethod
    def bandpass_filter(signal, lowcut, highcut, fs, order=5):
        nyq = 0.5 * fs
        low = lowcut / nyq
        high = highcut / nyq
        b, a = butter(order, [low, high], btype='band')
        y = lfilter(b, a, signal)
        return y

    


