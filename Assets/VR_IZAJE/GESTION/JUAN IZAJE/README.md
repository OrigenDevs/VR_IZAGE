# JUAN — Espacio de trabajo

Aquí puedes crear tus propios archivos de gestión:
- Notas diarias
- Listas de tareas
- Documentación técnica que generes
- Cualquier cosa que te ayude en tu día a día

## Formato sugerido
Usa `.md` (Markdown) para que sea legible desde Open Code y desde cualquier editor.

## Archivos útiles ya existentes
- `GESTION/OPEN CODE/INDEX.md` → Guía de inicio del proyecto
- `GESTION/OPEN CODE/DOCUMENTACION.md` → Documentación completa
- `GESTION/OPEN CODE/REPORTE.txt` → Reporte diario (actualiza aquí tu avance)
- `GESTION/DAVID/Progreso/Dashboard.html` → Dashboard visual del proyecto
- `GESTION/DAVID/Progreso/Diario.txt` → Historial de trabajo de David

## Recuerda
- Al empezar tu día: abre `REPORTE.txt` y escribe qué vas a hacer
- Al terminar: actualiza `REPORTE.txt` con lo que hiciste, problemas, próximos pasos
- Haz commits con mensajes claros: `[Área] Descripción`

---

## 16/06/2026 — Juan

### Qué hice hoy
- Agregué Terrain con heightmaps personalizados para el escenario de campo
- Optimicé y añadí modelo del camión (Camion/) con sus materiales
- Creé escena CAMION.unity para pruebas del vehículo
- Integré HDRI skies (kloofendal, rosendal) y material de ambiente
- Configuré paquete EasyRoads3D para carreteras en el terreno
- Ajustes en project settings y URP para soportar terrain/shaders

### Próximos pasos
- Integrar el camión en la escena principal de campo
- Colocar la grúa y el resto de la escenografía sobre el terrain

---

## 17/06/2026 — Juan

### Qué hice hoy
- Reemplacé modelo antiguo del camión por CamionPrincipal optimizado + Fence
- Añadí ADG_Textures (paquete de texturas para el terreno)
- Añadí ALP_Assets (nuevos assets para la escena)
- Añadido Forst (follaje/árboles para el terreno)
- Configuradas nuevas capas de terreno: Dirt, HalfDirt, Scree
- Ajustes en EasyRoads3D road material y terrain layers
- Agregados scripts Cs/ en la carpeta de gestión

### Próximos pasos
- Integrar el camión en la escena principal de campo (TRABAJO DE CAMPO IZAJE)
- Colocar follaje y texturas en el terrain
- Probar flujo en modo escritorio
