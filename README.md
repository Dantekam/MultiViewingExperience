# MultiViewingExperience

MultiViewingExperience is a Unity-based multiplayer VR application designed to allow users to experience immersive 360° video together across Meta Quest and desktop platforms.

The project builds upon the Ubiq networking framework developed by University College London (UCL) to provide multiplayer room management, synchronized video playback, networked avatars, voice communication, and shared user presence.

The project originally began as a Virtual Antarctic Exploration experience for viewing 360° Antarctic footage delivered by a professor at UNCW, but has since been expanded into a more general platform capable of supporting multiple immersive videos and environments.

## Features

- Multiplayer VR and desktop sessions
- Public and private multiplayer rooms
- Private room joining through generated room codes
- Host/client session management
- Host-controlled 360° video selection
- Synchronized video playback between connected users
- Networked avatars and avatar customization
- Player display names
- Voice communication between users
- Meta Quest support
- Desktop testing and participation
- Local video loading on Meta Quest devices
- Persistent video selection across multiplayer state changes

## Technologies

- Unity 2022.3 LTS
- C#
- Meta Quest 3
- XR Interaction Toolkit
- Ubiq Networking
- Android / Meta Quest deployment
- Unity VideoPlayer
- 360° equirectangular video
- Videos produced via Insta360 X5

## Multiplayer Architecture

MultiViewingExperience uses a host-controlled model for shared video experiences.

Users can create or join multiplayer rooms using Ubiq's networking system. A user may host a session and select a 360° video from the available video library. The selected video and its playback state are then synchronized with the connected clients so that participants experience the same content together.

The application also uses Ubiq's networking capabilities for shared avatars, display names, room management, and voice communication.

## 360° Video Playback

Videos are displayed as immersive 360° environments using Unity's video and rendering systems.

On Meta Quest, large video files can be stored separately from the application rather than packaged directly into the APK. This allows the application to locate and load locally stored video content at runtime while keeping the application build itself smaller.

The project is currently being expanded to improve video preparation, loading feedback, synchronization, and transitions between videos.

## Current Development

Current development includes:

- Improving persistence of selected videos between application states
- Preventing playback until video preparation has completed
- Adding clearer video-loading feedback
- Improving multiplayer room leave/reset behavior
- Refining avatar selection and default avatars
- Adding player video-status indicators
- Testing synchronization across multiple Meta Quest headsets

## Project Background

This project began as **Virtual Antarctic Exploration**, an application for experiencing immersive Antarctic 360° footage in VR.

As development expanded to support additional footage and more general multiplayer viewing scenarios, the project was renamed **MultiViewingExperience**.

The broader goal is to create a reusable platform in which multiple users can join the same immersive session, communicate with one another, and experience synchronized 360° media together from a dynamic state that allows videos to be added, prepared, and watched without manually increasing a list size to include such videos.

## Credits and Dependencies

### Ubiq

Multiplayer networking functionality in this project is built using **Ubiq**, an open-source Unity networking framework developed and maintained by the Virtual Environments and Computer Graphics group at University College London (UCL).

Ubiq provides networking infrastructure used by this project, including room management, networked objects, avatars, synchronization, and voice communication.

Ubiq Repository:
https://github.com/UCL-VR/ubiq

Ubiq Documentation:
https://ucl-vr.github.io/ubiq/

This project extends Ubiq's networking functionality with application-specific systems for synchronized 360° video selection and playback, video management, multiplayer session behavior, and the MultiViewingExperience user experience.

## Development Status

**Active Development**

MultiViewingExperience is currently a research/development project and may contain experimental features or unfinished functionality.

## Project Leadership

**Developer:** Kian Miley  
**Faculty Advisor:** Dr. Pedro Acevedo

Computer Science / Software Engineering  
University of North Carolina Wilmington
