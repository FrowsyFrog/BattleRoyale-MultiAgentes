# Trabajo Final - Tópicos en Ciencias de la Computación - The Great Agents
### Integrantes:
- Flores Tenorio, Juan Diego Enrique - u202014558
- Galindo Alvarez, Franco - u202010807
- Goyas Ayllón, Leonardo Andre - u202010206
### Profesor:
- Canaval Sanchez, Luis Martin
## Definición del problema y motivación
Antes de poder establecer una situación problemática, debemos entender qué es sistema multiagente. Signh (1994) afirma que son aquellos en los que resulta útil tomar una postura intencional, o aquellos de los que se puede decir que tiene un nivel de conocimiento distinto. Por su parte, Dorri, Kanhere y Jurdak (2018) señalan que un sistema multiagente consiste de un entidades autónomas que trabajan colaborativamente para resolver tareas flexiblemente gracias a su capacidad de inherente de aprender y tomar decisiones autónomas.

Partiendo de la premisa que un sistema multiagente es capaz de aprender y tomar decisiones, nuestro proyecto propone una investigación académica de las capacidades cognitivas de un grupo de agentes en un entorno cambiante. Este conjunto de entidades estará sometido a un ambiente conocido como *Battle Royale*, donde cada uno deberá recoger munición, esquivar explosivos enemigos y colocar sus propias bombas con el objetivo de ser el último al final de la contienda. Según sus acciones, un agente recibirá una recompensa postiva al esquivar un explosivo, recoger munción, acabar con un enemigo o ser el ganador de una contienda; y una recompensa negativa al perder, pasar un frame dentro del rango de una explosión o chocarse contra una pared de la arena.

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
- Unity: Plataforma de desarrollo de softwares multiplataforma.
- Lenguaje de programación: Consideramos el uso de C# para la creación del comportamiento de los agentes.

## Implementación
Hemos considerado los siguientes componentes de nuestro código fuente como los más relevantes de la investigación.

*Figura 1. Función CollectObservations. Elaboración propia, 2023.*

![image](https://github.com/FrowsyFrog/BattleRoyale-MultiAgentes/assets/91223158/74dbc19a-967b-4ecd-a09b-803bd7315204)

Como se observa en la Figura 1, hemos creado una función que permite al agente recibir información sobre el estatus actual de la contienda, incluyendo su propia posición, la cantidad de bombas disponbibles y evaluar si ha sido eliminado.

*Figura 2. Función OnActionReceived. Elaboración propia, 2023.*
![image](https://github.com/FrowsyFrog/BattleRoyale-MultiAgentes/assets/91223158/b91d3647-4843-4472-b3e3-d4c49d72756d)
![image](https://github.com/FrowsyFrog/BattleRoyale-MultiAgentes/assets/91223158/94ae8bde-b98d-45e2-864f-82ccd6950f47)

Como se observa en la Figura 2, hemos diseñado una función que permite a los agentes interactuar con el entorno. Incluye la intención de movimiento, la disponibilidad e intención de colocar una bomba y le otorga al agente la capacidad de pericibir si se encuentra dentro del rango de explosión de una bomba. 

*Video 1. Representación del aprendizaje de un agente. Elaboración propia, 2023.*
<video src="https://github.com/FrowsyFrog/Topicos-Agentes/assets/91223158/f9505d31-3f9f-426d-ba3f-f281d2707de7"></video>

*Figura 3. Lista de recompensas de un agente.*

![image](https://github.com/FrowsyFrog/BattleRoyale-MultiAgentes/assets/91223158/ad634a5b-896f-4e28-9e8b-078bea05d49b)

Como se observa en la Figura 3, se han diseñado diversas recompensas para los agentes en función de su comportamiento:
- WinReward: Recompensa positiva de un agente al ganar una contienda.
- KillReward: Recompensa positiva de un agente al eliminar otro agente.
- LosePenality: Recompensa negativa de un agente al perder una contienda.
- GrabAmmoReward: Recompensa positiva de un agente al recoger munición.
- InsideFutureExplosionPenality: Recompensa negativa de un agente por cada frame que permanezca dentro del radio de una bomba por explotar.
- OutFutureExplosionReward: Recompensa positiva de un agente por salir del rango de una explosión.
- OnWallPenality: Recompensa negativa de un agente por cada frame que colisiones contra una pared.

Además, como parte de nuestra implementación, diseñamos un diagrama para ilustrar la conexión de las clases más relevantes del proyecto.

*Figura 4. Diagrama de Clases de Battle Royale Multiagentes.* 
![image](https://github.com/FrowsyFrog/BattleRoyale-MultiAgentes/assets/91223158/6026a2e8-e5fa-4b88-b10c-a4d75e0c8d27)


## Resultados

El desarrollo del proyecto tuvo como resultado la creación de un sistema con dos modos.

Por un lado, el modo Headless abre una consola donde se muestra la información relevante de lo que está ocurriendo en el *Battle Royale*。 Este modo permite compilar el sistema con mínima exigencia de recursos, maximizando su velocidad.

*Figura 5. Interfaz en modo Headless para la computación rápida del sistema*
![image](https://github.com/FrowsyFrog/BattleRoyale-MultiAgentes/assets/91223158/3d022f15-84bc-4ec3-98f9-a8a0abe4c518)

Por otro lado, el modo Interfaz visual abre una aplicación donde se muestran los modelos de los agentes, las bombas, munición y otros detalles del proyecto. Este modo, permite una experiencia más cómoda para el usuario, sacrificando cierto costo computacional.

*Vídeo 2. Prueba del proyecto con interfaz visual*
<video src="https://github.com/FrowsyFrog/BattleRoyale-MultiAgentes/assets/91223158/87de0b2c-eba8-4eb0-b864-bb8a999b5672"></video>

## Conclusiones

- La modelación del entorno simulado fue capaz de albergar múltiples agentes y entidades simultáneamente gracias a las diversas herramientas disponibles en las plataformas utilizadas.
- La interfaz visual proveida por Unity tuvo un buen desempeño, permitiendo observar el entorno sin interrupciones.
- El entrenamiento de los agentes fue validado mediante pruebas lúdicas, mostrando que su funcionamiento es óptimo.
- Las herramientas designadas para el desarrollo del sistema multiagente demostraron ser efectivas, debido a que permitieron la implementación exitosa de los requisitos solicitados para el proyecto.

### Bibliografía
Dorri, A., Kanhere, S. S., & Jurdak, R. (2018). Multi-agent systems: A survey. *Ieee Access*, 6, 28573-28593. https://doi.org/10.1109/ACCESS.2018.2831228

Singh, M. P. (1994). Multiagent systems. In J. G. Carbonell & J. Siekmann (Ed.)., *Multiagent systems* (pp. 1-2). https://doi.org/10.1007/BFb0030532
