from .das_enviroment import DasEnviroment
from . import helpers

class DasSignal:
    #load the signal from file
    def __init__(self, signal, env = DasEnviroment()):
        self.env = env
        self.signal = signal        

    @staticmethod
    def load_from_file(file_path_name, env = DasEnviroment()):
        return DasSignal(helpers.load_das_mat(file_path_name), env)
    
    def get_signal(self):
        return self.signal
    
    def get_env(self):
        return self.env
    
    #apply a filter to the signal
    def apply_filter(self, filter):
        self.signal = filter.apply(self.signal)


    

    

    
        

