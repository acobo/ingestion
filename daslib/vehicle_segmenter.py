import numpy as np
class VehicleSegmenter:
    def __init__(self, signal):        
        self.signal = signal
        pass
    
    def process(self, signal):
        pass

    def smooth_signal(self, signal, sigma):
        return gaussian_filter1d(signal, sigma)

    def numerical_derivative(self, signal, dt):
        return np.gradient(signal, dt)

    def detect_vehicles_from_time_series(self, time_series, threshold, min_samples_between_vehicles, sigma, dt, derivative_threshold):
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
                vehicle_count += 1
                start_position = i
            elif value < threshold and np.abs(derivative) < derivative_threshold and detected_vehicle:
                detected_vehicle = False
                end_position = i
                vehicle_positions.append((start_position, end_position))
            else:
                sample_count_since_last_vehicle = 0

        return vehicle_count, vehicle_positions




    