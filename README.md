# Trabajo Final - Tópicos en Ciencias de la Computación - The Great Agents
### Integrantes:
- Flores Tenorio, Juan Diego Enrique - u202014558
- Galindo Alvarez, Franco - u202010807
- Goyas Ayllón, Leonardo Andre - u202010206
### Profesor:
- Canaval Sanchez, Luis Martin
## Definición del problema y motivación
Antes de poder establecer una situación problemática, debemos entender qué es sistema multiagente. Signh (1994) afirma que son aquellos en los que resulta útil tomar una postura intencional, o aquellos de los que se puede decir que tiene un nivel de conocimiento distinto. Por su parte, Dorri, Kanhere y Jurdak (2018) señalan que un sistema multiagente consiste de un entidades autónomas que trabajan colaborativamente para resolver tareas flexiblemente gracias a su capacidad de inherente de aprender y tomar decisiones autónomas.

Partiendo de la premisa que un sistema multiagente es capaz de aprender y tomar decisiones, nuestro proyecto propone una investigación académica de las capacidades cognitivas de un grupo de agentes en un entorno cambiante. Este conjunto de entidades estará sometido a un ambiente conocido como *Battle Royale*, donde cada uno deberá esquivar explosivos con el objetivo de ser el último al final de la contienda. Según sus acciones, un agente recibirá una recompensa postiva al esquivar un explosivo; caso contrario, recibirá una recompensa negativa.
La idea para este proyecto surge de la popularidad de este género en la industria de los videojuegos, combinada con nuestro interés por la evolución de un agente en un contexto caótico.

## Objetivos
### Objetivo General:
Desarrollar un sistema multiagente basado en *Battle Royale* que permita a los agentes evolucionar sus capacidades adaptativas.
### Objetivos Específicos:
- Modelar un entorno simulado de *Battle Royale* capaz de albergar múltiples agentes.
- Elaborar una interfaz visual para visibilizar la evolución de los agentes.
- Validar el funcionamiento del sistema basándonos en las estrategias que adoptan los agentes.

## Metodología
Con el objetivo de desarrollar el sistema multiagente propuesto, hemos considerado utilizar la siguiente configuración:
- Pytorch: Biblioteca de Python que incluye diferentes métodos para el desarrollo de aprendizaje automático.
- CUDA: Plataforma de computación paralela que permite la codificación de algoritmos en GPU de Nvidia. Cuenta con un compilador y herramientas de desarrollo.
- ML Agents: Software open-source de creación para la creación de personajes virtuales.
- Unity: Plataforma de desarollo de softwares multiplataforma.
- Lenguaje de programación: Consideramos el uso de C# para la creación del comportamiento de los agentes.

## Implementación
Hasta la fecha de realización del informe, hemos desarrollado una implementación parcial de las funcionalidades previstas.

*Figura 1. Función CollectObservations. Elaboración propia, 2023.*
![image](https://github.com/FrowsyFrog/Topicos-Agentes/assets/91223158/e7869d39-16b7-4e4f-9f29-f2f88cdfb701)

Como se observa en la Figura 1, hemos creado una función que permite al agente recibir las observaciones de cuatro variables:
- Posición: Representado por un vector de tres dimensiones por cada agente.
- Posición de las bombas: Representado por un vector de tres dimensiones por cada bomba.
- Tiempo restante para la explosión de una bomba: Representado por un punto flotante.
- Estado de una bomba: Representado por un entero de que puede tomar tres valores.

*Figura 2. Función OnActionReceived. Elaboración propia, 2023.*
![image](https://github.com/FrowsyFrog/Topicos-Agentes/assets/91223158/4cf5ba02-de32-422b-9191-d018ca6da8e5)

Como se observa en la Figura 2, hemos diseñado una función que permite a los agentes esquivar las bombas utilizando dos variables y un método:
- Dirección de próximo movimiento: Representado por dos flotantes en el rango [-1;1].
- Movimiento: Utilizando los valores de las variables, el deltaTime y la velocidad de un agente.

*Video 1. Representación del aprendizaje de un agente. Elaboración propia, 2023.*
<video src="https://github.com/FrowsyFrog/Topicos-Agentes/assets/91223158/f9505d31-3f9f-426d-ba3f-f281d2707de7"></video>

### Bibliografía
Dorri, A., Kanhere, S. S., & Jurdak, R. (2018). Multi-agent systems: A survey. *Ieee Access*, 6, 28573-28593. https://doi.org/10.1109/ACCESS.2018.2831228

Singh, M. P. (1994). Multiagent systems. In J. G. Carbonell & J. Siekmann (Ed.)., *Multiagent systems* (pp. 1-2). https://doi.org/10.1007/BFb0030532




