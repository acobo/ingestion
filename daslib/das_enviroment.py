#define class DasEnviroment with properties Ts, Tl
class DasEnviroment:
    def __init__(self, Ts=0.005, Tl=1):
        """Sampling time in msg"""
        self.Ts = Ts
        """Sampling length in meters"""
        self.Tl = Tl