# CLAUDE.md — `Assets/Addressables`

Read the root `CLAUDE.md` first. This file is folder-local, and it is the one to read **before
touching any UI string, key or table** — the failure mode here is silent and shows up as a key
rendered on screen.

## What is here

| Asset | Role |
|---|---|
| `Localization/Localization Settings.asset` | the settings object the runtime resolves through |
| `Localization/English (en).asset` | the locale |
| `Strings/Strings Shared Data.asset` | the **keys** and their metadata — the shared spine |
| `Strings/Strings.asset` | the table collection |
| `Strings/Strings_en.asset` | the **values** for `en` |

Target locales: <target locales>.

## A string lives in three places at once

Adding an entry means the key exists in the shared data **and** a value exists in every locale table
**and** the table itself is registered. Any one of those missing produces the same symptom — the key
renders instead of the text — and only one of the three is where you were looking.

This is the reason to edit strings through the Localization window rather than by hand: it keeps the
three in step. A hand-edit to one of these YAML files is the "small edit" case of
`.claude/rules/unity_assets.md` — allowed, reported, and verified by actually reading the other two.

## Key grammar

Keys are `snake_case`, and they name **where the string appears and what it says**, not the text
itself: `boot_screen_title`, `settings_audio_master_label`. A key derived from the English wording
becomes a lie the first time the wording changes, and nobody renames keys.

Group by screen or subsystem first, specific element last. Keep the prefix in step with the screen's
own name — when a screen is renamed, its keys are renamed with it, in the same change.

## Addressables

Being in the project, or in the build settings, is **not** the same as being shipped: an asset that
belongs to no Addressables group is simply absent at runtime, with no build-time complaint. This
catches scenes most often — `Assets/Scenes/Boot.unity` is in the build settings, which is a different
mechanism.

Group layout, bundle packing and the profile live in `Assets/AddressableAssetsData/`. Changing how
content is packed is a decision with a build-size and load-time cost — it belongs in `Docs/adr.md`,
not in a passing edit.
