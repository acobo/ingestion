import numpy as np
import cv2
import matplotlib.pyplot as plt
import scipy.io
import pandas as pd
from scipy import signal
from scipy.ndimage import gaussian_filter1d
from scipy.signal import stft, find_peaks

#load mat and video file
def load_das_mat(matfn):
    '''Load Das mat file and return numpy array'''
    '''[t][length]'''
    #load mat file
    mat = scipy.io.loadmat(matfn)
    return mat["Fp"]

def smooth_signal(signal, sigma):
    return gaussian_filter1d(signal, sigma)

def numerical_derivative(signal, dt):
    return np.gradient(signal, dt)

def signal_energy(signal):
    return np.sum(signal ** 2)

def count_and_separate_vehicles(deformation_signals, threshold, min_samples_between_vehicles):
    vehicle_count = 0
    detected_vehicle = False
    sample_count_since_last_vehicle = 0
    separated_vehicle_signals = []

    for i in range(deformation_signals.shape[1]):
        current_column = deformation_signals[:, i]
        max_deformation = np.max(current_column)

        if max_deformation >= threshold and not detected_vehicle:
            detected_vehicle = True
            vehicle_count += 1
            separated_vehicle_signals.append([])
        
        if detected_vehicle:
            separated_vehicle_signals[-1].append(current_column)
            sample_count_since_last_vehicle += 1

            if max_deformation < threshold and sample_count_since_last_vehicle >= min_samples_between_vehicles:
                detected_vehicle = False
                sample_count_since_last_vehicle = 0

    # Convierte las listas de señales separadas en arrays de numpy
    separated_vehicle_signals = [np.column_stack(vehicle_signal) for vehicle_signal in separated_vehicle_signals]

    return vehicle_count, separated_vehicle_signals

def detect_vehicles_from_time_series(time_series, threshold, min_samples_between_vehicles, sigma, dt, derivative_threshold, energy_threshold):
    # Suavizar la señal
    smoothed_signal = smooth_signal(time_series, sigma)

    # Calcular la derivada numérica
    derivative_signal = numerical_derivative(smoothed_signal, dt)

    # Contar vehículos usando la señal suavizada y su derivada
    vehicle_count = 0
    detected_vehicle = False
    sample_count_since_last_vehicle = 0
    vehicle_positions = []

    for i, (value, derivative) in enumerate(zip(smoothed_signal, derivative_signal)):
        if value >= threshold and np.abs(derivative) > derivative_threshold and not detected_vehicle:
            detected_vehicle = True
            start_position = i
        elif value < threshold and np.abs(derivative) < derivative_threshold and detected_vehicle:
            detected_vehicle = False
            end_position = i
            vehicle_signal = time_series[start_position:end_position]
            vehicle_energy = signal_energy(vehicle_signal)

            if vehicle_energy > energy_threshold:
                vehicle_positions.append((start_position, end_position))
                vehicle_count += 1
        else:
            sample_count_since_last_vehicle = 0

    return vehicle_count, vehicle_positions

def detect_vehicles_advanced(time_series, threshold_factor, window_size, noverlap, min_samples_between_vehicles):
    # Calcular la STFT
    f, t, Zxx = stft(time_series, nperseg=window_size, noverlap=noverlap)

    # Calcular el espectro de potencia
    power_spectrum = np.abs(Zxx)**2

    # Calcular el umbral adaptativo
    adaptive_threshold = threshold_factor * np.mean(power_spectrum, axis=0)

    # Encontrar picos en el espectro de potencia
    peaks, _ = find_peaks(np.mean(power_spectrum, axis=0), height=adaptive_threshold, distance=min_samples_between_vehicles)

    # Contar vehículos y determinar posiciones
    vehicle_count = len(peaks)
    vehicle_positions = [(t[p], t[p+1]) for p in peaks[:-1]]

    return vehicle_count, vehicle_positions


#bandpass filter
def bandpass_filter(data, lowcut, highcut, fs, order=5):
    nyq = 0.5 * fs
    low = lowcut / nyq
    high = highcut / nyq
    b, a = signal.butter(order, [low, high], btype='band')
    y = signal.lfilter(b, a, data)
    return y

import numpy as np
import scipy.signal
import matplotlib.pyplot as plt

def stft_spectrogram(signal, fs=1, window='hann', nperseg=256, noverlap=None):
    """
    Calcula la STFT y el espectrograma de una señal 2D utilizando scipy.signal.

    Parámetros:
    signal (np.array): Array 2D de numpy con las dimensiones de tiempo y longitud.
    fs (int, opcional): Frecuencia de muestreo de la señal (por defecto 1).
    window (str, opcional): Tipo de ventana utilizada para la STFT (por defecto 'hann').
    nperseg (int, opcional): Número de puntos por segmento en la STFT (por defecto 256).
    noverlap (int, opcional): Número de puntos de solapamiento entre segmentos (por defecto None, que corresponde a nperseg // 8).

    Retorna:
    f (np.array): Array de frecuencias.
    t (np.array): Array de tiempos.
    Zxx (np.array): Array 2D de la STFT (espectrograma).
    """
    if noverlap is None:
        noverlap = nperseg // 8

    f, t, Zxx = scipy.signal.stft(signal, fs=fs, window=window, nperseg=nperseg, noverlap=noverlap)
    return f, t, Zxx

# Ejemplo de uso
#signal = np.random.rand(1000, 100)  # Reemplaza esto con tu señal DAS
#fs = 100  # Frecuencia de muestreo (ajústala según tus datos)

#f, t, Zxx = stft_spectrogram(signal, fs=fs)

# Visualización del espectrograma
# plt.pcolormesh(t, f, np.abs(Zxx), shading='gouraud')
# plt.title('Espectrograma')
# plt.xlabel('Tiempo [s]')
# plt.ylabel('Frecuencia [Hz]')
# plt.show()

# import numpy as np
# import scipy.signal
# import matplotlib.pyplot as plt

def adaptive_filter_peak_detection(signal, window_len=51, polyorder=3, threshold=0.5):
    """
    Aplica un filtro adaptativo (Savitzky-Golay) y detecta picos en una señal 1D.

    Parámetros:
    signal (np.array): Array 1D de numpy que contiene la señal.
    window_len (int, opcional): Tamaño de la ventana del filtro Savitzky-Golay (por defecto 51).
    polyorder (int, opcional): Orden del polinomio utilizado en el filtro Savitzky-Golay (por defecto 3).
    threshold (float, opcional): Umbral para la detección de picos (por defecto 0.5).

    Retorna:
    filtered_signal (np.array): Señal filtrada con el filtro Savitzky-Golay.
    peak_indices (np.array): Índices de los picos detectados en la señal filtrada.
    """
    # Aplicar el filtro Savitzky-Golay
    filtered_signal = scipy.signal.savgol_filter(signal, window_length=window_len, polyorder=polyorder)

    # Detectar picos en la señal filtrada
    peak_indices = scipy.signal.find_peaks(filtered_signal, height=threshold)[0]

    return filtered_signal, peak_indices

# Ejemplo de uso
# signal = np.random.rand(1000)  # Reemplaza esto con un array 1D de tu señal DAS
# filtered_signal, peak_indices = adaptive_filter_peak_detection(signal)

# Visualización de la señal original, señal filtrada y picos detectados
# plt.plot(signal, label='Señal original')
# plt.plot(filtered_signal, label='Señal filtrada', linestyle='--')
# plt.scatter(peak_indices, filtered_signal[peak_indices], color='red', marker='o', label='Picos detectados')
# plt.legend()
# plt.xlabel('Tiempo')
# plt.ylabel('Amplitud')
# plt.show()

# import numpy as np

def das_beamforming(data, sensor_positions, directions, speed_of_light=3e8):
    """
    Aplica beamforming a datos DAS en un arreglo lineal de sensores.

    Parámetros:
    data (np.array): Array 2D de numpy con las dimensiones de tiempo y sensores.
    sensor_positions (np.array): Array 1D con las posiciones de los sensores en el arreglo.
    directions (np.array): Array 1D con las direcciones de interés para el beamforming (en radianes).
    speed_of_light (float, opcional): Velocidad de la luz en m/s (por defecto 3e8).

    Retorna:
    beamformed_data (np.array): Array 2D con las dimensiones de tiempo y direcciones, resultado del beamforming.
    """
    num_sensors = len(sensor_positions)
    num_directions = len(directions)
    num_time_steps = data.shape[0]

    beamformed_data = np.zeros((num_time_steps, num_directions))

    for i, direction in enumerate(directions):
        # Calcular los retardos de tiempo para cada sensor en función de la dirección
        time_delays = (sensor_positions / speed_of_light) * np.sin(direction)

        # Aplicar los retardos y sumar las señales de todos los sensores
        delayed_signals = np.zeros_like(data)
        for sensor_idx in range(num_sensors):
            delayed_signals[:, sensor_idx] = np.roll(data[:, sensor_idx], int(time_delays[sensor_idx]))

        # Calcular la salida del beamforming para esta dirección
        beamformed_data[:, i] = np.sum(delayed_signals, axis=1)

    return beamformed_data

# Ejemplo de uso
# data = np.random.rand(1000, 10)  # Reemplaza esto con tus datos DAS
# sensor_positions = np.linspace(0, 10, 10)  # Posiciones de los sensores en metros
# directions = np.linspace(-np.pi / 2, np.pi / 2, 180)  # Direcciones de interés en radianes

# beamformed_data = das_beamforming(data, sensor_positions, directions)

def signal_energy(signal):
    return np.sum(np.square(signal))

def cross_correlation(signal1, signal2):
    return np.correlate(signal1, signal2, mode='valid')

def compute_similarity(signal1, signal2, energy_weight=0.5, temporal_weight=0.5):
    # Normaliza las señales
    signal1_normalized = signal1 / np.linalg.norm(signal1)
    signal2_normalized = signal2 / np.linalg.norm(signal2)
    
    # Calcula la energía de las señales
    energy1 = signal_energy(signal1_normalized)
    energy2 = signal_energy(signal2_normalized)
    
    # Calcula la correlación cruzada de las señales
    correlation = cross_correlation(signal1_normalized, signal2_normalized)
    
    # Calcula la similaridad basándose en la energía y la correlación cruzada
    energy_similarity = 1 - abs(energy1 - energy2) / (energy1 + energy2)
    temporal_similarity = np.max(correlation) / (np.sqrt(energy1) * np.sqrt(energy2))
    
    # Combina las medidas de similaridad utilizando los pesos especificados
    similarity = energy_weight * energy_similarity + temporal_weight * temporal_similarity
    
    return similarity