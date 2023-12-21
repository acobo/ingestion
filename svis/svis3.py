import sys
import math
import os
sys.path.insert(1, os.path.join(sys.path[0], '..'))
import dearpygui.dearpygui as dpg
import numpy as np

from daslib import DasEnviroment, DasSignal, helpers, Processing1D, detect_vehicles_from_time_series


f_sampling = 200
current_index = 0
max_index = 0
mat = None
x_data = []
y_data = []
y_data2 = []

xt_data = []
yt_data = []
yt_data2 = []

plot_l = None

dasenv = DasEnviroment()

# crear una función para actualizar el plot según el tiempo del slider
def update_l_plot(sender, data):
    if mat is None:
        return
    # obtener el valor del slider
    index = dpg.get_value("slider_l1")
    index2 = dpg.get_value("slider_l2")
    # calcular el índice del dato más cercano al tiempo
    #index = int(time * 1000)
    # actualizar los datos del plot usando solo los valores hasta el índice
    #copy mat to x_data and y_data
    x_data = [x/f_sampling for x in range(0,mat.shape[0])]    
    y_data = mat[:,index].tolist()
    y_data2 = mat[:,index2].tolist()

    #square data if checkbox is checked
    if dpg.get_value("cb_square_filter"):
        y_data = [x**2 for x in y_data]
        y_data2 = [x**2 for x in y_data2]
    #bandpass filter yt_data if checkbox is checked
    if dpg.get_value("cb_bandpass_filter"):
        #y_data = Processing1D.bandpass_filter(y_data, 0.3, 3.5, f_sampling, 2)
        #y_data2 = Processing1D.bandpass_filter(y_data2, 0.3, 3.5, f_sampling, 2)
        y_data = Processing1D.bandpass_filter(y_data, 0.1, 2, f_sampling, 2)
        y_data2 = Processing1D.bandpass_filter(y_data2, 0.1, 2, f_sampling, 2)
    
    dpg.fit_axis_data("x_axis")
    dpg.set_value("line_series_l1", [x_data, y_data])        
    dpg.set_value("line_series_l2", [x_data, y_data2])     

    #detectamos vehiculos en y_data
    dt = 1/200
    threshold = 0.055
    sigma = 5
    #numpy array from y_data
    y_data_np = np.array(y_data)
    vehicle_count, vehicle_positions = detect_vehicles_from_time_series(y_data_np, threshold, 1, sigma, dt,0.01,0.001)    
               # Añadir tramos detectados
    #get plot    
    max_y = np.max(y_data)
    min_y = np.min(y_data)

    children_ids = dpg.get_item_children("y_axis")    
    for child_id in children_ids[1]:
        if dpg.get_item_type(child_id) == "mvAppItemType::mvShadeSeries":
            dpg.delete_item(child_id)
    
    #pack all tuples in a list
    # vehicle_positionsll = [item for sublist in vehicle_positions for item in sublist]    
    # #divide by f_sampling
    # vehicle_positionsll = [x/f_sampling for x in vehicle_positionsll]
    # #create array of length vehicle_positionsll with max_y
    # max_y_array = [max_y for x in range(0,len(vehicle_positionsll))]
    # dpg.add_shade_series(vehicle_positionsll, max_y_array, parent="y_axis")

    for start, end in vehicle_positions:
        shadeitem = dpg.add_shade_series([start/200, (end)/200], [max_y, max_y], parent="y_axis")        
        dpg.bind_item_theme(shadeitem, "plot_theme")
    # try:        
    #     for start, end in vehicle_positions:
    #         x = np.arange(start, end) / f_sampling
    #         y1 = max_y
    #         y2 = max_y
    #         #dpg.add_area_series([start/200, (end)/200], [0, max_y], parent="y_axis", color=(255, 0, 0, 100))
    #         dpg.draw_polyline([[start/200, 0], [start/200, max_y]], parent="y_axis")

    #catch and print error msg
    # except Exception as e:
    #     print(e)

    print("vehicle_count: ", vehicle_count)




def update_t_plot(sender, data):
    if mat is None:
        return
    # obtener el valor del slider
    index = dpg.get_value("slider_t1")
    index2 = dpg.get_value("slider_t2")
    # calcular el índice del dato más cercano al tiempo
    index = int(index * f_sampling)
    index2 = int(index2 * f_sampling)
    # actualizar los datos del plot usando solo los valores hasta el índice
    #copy mat to x_data and y_data
    xt_data = [x for x in range(0,mat.shape[1])]    
    yt_data = mat[index,:].tolist()
    yt_data2 = mat[index2,:].tolist()
    
    dpg.fit_axis_data("xt_axis")
    dpg.set_value("line_series_t1", [xt_data, yt_data])        
    dpg.set_value("line_series_t2", [xt_data, yt_data2])

def configure_sliders():
    if mat is None:
        return
    
    #configure slider max value to mat.shape[1]
    dpg.configure_item("slider_l1", max_value=mat.shape[1]-1)
    dpg.configure_item("slider_l2", max_value=mat.shape[1]-1)
    #configure slider max value to mat.shape[0]
    dpg.configure_item("slider_t1", max_value=(mat.shape[0]-1)/f_sampling)
    dpg.configure_item("slider_t2", max_value=(mat.shape[0]-1)/f_sampling)


def save_callback():
    print("Save Clicked")

def load_mat_callback(sender, app_data, user_data):
    global mat
    print(app_data)
    mat = helpers.load_das_mat(app_data['file_path_name'])    
    print(mat.shape)
    print("load_mat_callback2")
    configure_sliders()

#load mat callback
def show_file_dialog_callback(sender, app_data):    
    #create file dialog
    if (dpg.does_item_exist("file_dialog_id")):
        dpg.delete_item("file_dialog_id")
    with dpg.file_dialog(directory_selector=False, show=False, callback=load_mat_callback,
                          tag="file_dialog_id", width=700 ,height=400,modal=True,
                          default_path="E:/das/2021-11-30_Medidas", label="Load mat file"):
        dpg.add_file_extension(".mat", color=(0, 255, 0, 255), custom_text="[Das mat file]")
    dpg.show_item("file_dialog_id")
                        


dpg.create_context()
dpg.configure_app(manual_callback_management=True)
dpg.create_viewport(title="dvis", width=1600, height=900, resizable=False)
dpg.setup_dearpygui()
#dpg.show_debug()

with dpg.window(tag="Primary Window", label="dvis", width=1600, height=900, no_resize=True, no_move=True, no_close=True, no_title_bar=True):
    with dpg.theme(tag="plot_theme"):
        with dpg.theme_component(dpg.mvShadeSeries, parent="plot_theme"):
            dpg.add_theme_color(dpg.mvPlotCol_Fill, (0, 255, 0, 128), category=dpg.mvThemeCat_Core)                  
    

    #create main menu bar
    with dpg.menu_bar():
        #create file menu
        with dpg.menu(label="File"):
            #load mat file
            dpg.add_menu_item(label="Load mat", callback=show_file_dialog_callback)            
            dpg.add_menu_item(label="Save", callback=save_callback)
            dpg.add_menu_item(label="Exit", callback=dpg.stop_dearpygui)
        #create help menu
        with dpg.menu(label="Help"):
            dpg.add_menu_item(label="About")       
        #create filter menu
        with dpg.menu(label="Filter"):
            #checkbox bandpass filter
            dpg.add_checkbox(label="Bandpass filter", tag="cb_bandpass_filter", default_value=False, callback=update_l_plot)         
            #checkbox square filter
            dpg.add_checkbox(label="Square filter", tag="cb_square_filter", default_value=False, callback=update_l_plot)

    #create two tabs
    with dpg.tab_bar():
        with dpg.tab(label="Distancia"):            
             # crear un plot con un eje x y un eje y y con max width              
            with dpg.group(label="Distancia"):
                # crear un slider para controlar el tiempo         
                dpg.add_slider_int(label="Longitud", tag="slider_l1", default_value=0,
                                min_value=0, max_value=100, width=-1,
                                callback=update_l_plot)
                dpg.add_slider_int(label="Longitud 2", tag="slider_l2", default_value=0,
                                min_value=0, max_value=100, width=-1,
                                callback=update_l_plot)
                with dpg.plot(label="Plot", height=-1, width=-1, tag="plot_l"):                
                    dpg.add_plot_legend()
                    dpg.add_plot_axis(dpg.mvXAxis, label="tiempo (sg)", tag="x_axis")       
                    dpg.set_axis_limits_auto("x_axis")         
                    with dpg.plot_axis(dpg.mvYAxis, 
                    label="Perturbación", tag="y_axis", ):
                        # agregar una serie de datos al eje y usando los datos generados
                        # asignarle una etiqueta única para poder referenciarla luego
                        dpg.add_line_series(x_data, y_data, label="Perturbación l1", tag="line_series_l1")                    
                        dpg.add_line_series(x_data, y_data2, label="Perturbación l2", tag="line_series_l2")                    
                        dpg.set_axis_limits("y_axis", -1, 1)                        

        with dpg.tab(label="Tiempo"):            
             # crear un plot con un eje x y un eje y y con max width              
            with dpg.group(label="Tiempo"):
                # crear un slider para controlar el tiempo         
                dpg.add_slider_float(label="Tiempo", tag="slider_t1", default_value=0,
                                min_value=0, max_value=100, width=-1,
                                callback=update_t_plot)
                dpg.add_slider_float(label="Tiempo2 2", tag="slider_t2", default_value=0,
                                min_value=0, max_value=100, width=-1,
                                callback=update_t_plot)
                with dpg.plot(label="Plot tiempo", height=-1, width=-1):
                    dpg.add_plot_legend()
                    dpg.add_plot_axis(dpg.mvXAxis, label="distancia (m)", tag="xt_axis")       
                    dpg.set_axis_limits_auto("x_axis")         
                    with dpg.plot_axis(dpg.mvYAxis, label="perturbación", tag="yt_axis", ):
                        # agregar una serie de datos al eje y usando los datos generados
                        # asignarle una etiqueta única para poder referenciarla luego
                        dpg.add_line_series(xt_data, yt_data, label="Perturbación t1", tag="line_series_t1")                    
                        dpg.add_line_series(xt_data, yt_data2, label="Perturbación t2", tag="line_series_t2")                    
                        dpg.set_axis_limits("yt_axis", -1, 1)    
    

dpg.show_viewport()
dpg.set_primary_window("Primary Window", True)
#dpg.start_dearpygui()
while dpg.is_dearpygui_running():
    jobs = dpg.get_callback_queue() # retrieves and clears queue
    dpg.run_callbacks(jobs)
    dpg.render_dearpygui_frame()

dpg.destroy_context()