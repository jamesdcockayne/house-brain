[![Docker Image CI](https://github.com/jamesdcockayne/house-brain/actions/workflows/CI.yml/badge.svg)](https://github.com/jamesdcockayne/house-brain/actions/workflows/CI.yml)

# Heat and cool

Manages hot water heating and solar generation.

# Hardware

* epaper display https://www.waveshare.com/wiki/E-Paper_Driver_HAT
* 5 tank temp sensors (from top to bottom)
* gas heating inlet and outlet sensor
* cold water tank temp sensor
* relay to switch between upper and lower immersion element
* relay to call for boiler heat

# Interface

* Button to call or cancel gas heat
* Another button - perhaps to display more details on the display

# Display

Display lists the following information

* Number of liters of hot water
> This will be calculated using the tank sensors, and incoming water temperature
* Status of electric heating
* * Number of watts going into the tank
* * Indicate which element is active
* * Number of kWh so far today
* Status of gas heating
* * Minutes of heating
* * Inlet outlet temps

# System events

* Incoming sofar MQTT reading
* Periodic display refresh
* Heat button press
* Other button press

# System routines

## Gas heating
Closes relay to call for boiler heat. Stops call for heat on any of the following conditions.
* Heating inlet and outlet temps are similar and hot - this means the water is no longer being heated
* The tank is at the desired temp (60c?)
* A fixed duration of time passes
* The call button was pressed again

## Immersion management

* Records when current is flowing to the elements. We need this to calculate the total number of kWh sent to the tank.
* Manages the switch between the top element to the bottom element once current is no longer detected to be flowing to the top element.
* * If current does not flow to the bottom element we will switch back to the top element because we may be in either of the following conditions
* * 1. The whole tank is 70c or above
* * 2. There is no excess power available to power either immersion
* * If power is flowing to the bottom element, periodically test if current will flow to the top element again, as it may have cooled.

# Architecture

Each component of the solution will run inside a docker container

## e-paper display docker image

A python web service, that uses pillow to render images to the e-paper display. The web service will accept images.

## postgress db

The database that records the following information

1. Sofar inverter readings (MQTT)
2. Temp sensor readings
3. Immersion current readings
4. Logs

## The service

* Receives MQTT messages from the ESP32 on the sofar inverter.
* Renders the e-paper display image
* Manages periodic and event based tasks.
* Probably serves the same information via a react app