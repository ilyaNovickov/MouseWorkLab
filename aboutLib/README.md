# Библиотека Dock для Avalonia — обзор возможностей

Этот каталог содержит материалы на русском языке по изучению возможностей
библиотеки [wieslawsoltes/Dock](https://github.com/wieslawsoltes/Dock) —
системы докинга (стационарных и плавающих панелей) для Avalonia UI.

Официальная документация:

- Репозиторий: https://github.com/wieslawsoltes/Dock
- Документация: https://wieslawsoltes.github.io/Dock/

## Что это такое

Dock — это система компоновки (layout) для приложений Avalonia. Она позволяет
строить интерфейсы, похожие на IDE: с вкладками документов, инструментальными
панелями (tools), перетаскиванием, плавающими окнами и сохранением раскладки.

Ключевые возможности:

- Документы и инструменты с настраиваемыми правилами докинга.
- Плавающие окна, цели докинга (docking targets) и жесты перетаскивания.
- Сохранение и восстановление раскладки (persistence).
- Поддержка тем и кастомизация оверлеев.
- Интеграции для MVVM, ReactiveUI, Prism и других фреймворков.
- Отложенное (deferred) отображение контента и переиспользование контролов
  (recycling) для производительности.

## Что можно строить

- Многодокументный интерфейс (MDI) с вкладками документов.
- Инструментальные панели, которые могут скрываться, закрепляться (pin) или
  всплывать (float).
- Сплит-раскладки с пропорциональным размером и перетаскиваемыми разделителями.
- Плавающие окна с индикаторами докинга.
- Управляемые плавающие окна внутри главного окна.
- Сохранённые раскладки, восстанавливаемые между сессиями.

## Содержание материалов

- [Архитектура и ключевые понятия](architecture.md)
- [Способы создания раскладки (code-first и ItemsSource)](layout-creation.md)
- [Методы наполнения документов и инструментов контентом](content-methods.md)
- [Плавающие окна, сложные раскладки и сохранение состояния](floating-windows-and-persistence.md)
- [Связь с вашим кодом (DockFactory и ViewLocator)](relate-to-your-code.md)

## NuGet-пакеты

Базовый набор для большинства сценариев:

```
dotnet add package Dock.Avalonia
dotnet add package Dock.Model.Mvvm
dotnet add package Dock.Avalonia.Themes.Fluent
```

Дополнительно (по необходимости):

- Сериализаторы: `Dock.Serializer.Newtonsoft`, `Dock.Serializer.SystemTextJson`,
  `Dock.Serializer.Protobuf`, `Dock.Serializer.Xml`, `Dock.Serializer.Yaml`.
- Темы: `Dock.Avalonia.Themes.Fluent`, `Dock.Avalonia.Themes.Browser`,
  `Dock.Avalonia.Themes.Simple`.
- Диагностика: `Dock.Avalonia.Diagnostics`.
- Контролы: `Dock.Controls.DeferredContentControl`, `Dock.Controls.Recycling`.
- Фреймворк-интеграции: `Dock.Model.ReactiveUI`, `Dock.Model.Prism` и др.
