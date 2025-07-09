/*
 * https://docs.gradle.org/8.11.1/userguide/multi_project_builds.html
 */

pluginManagement {
    repositories {
        gradlePluginPortal()
    }
}

plugins {
    // Apply the foojay-resolver plugin to allow automatic download of JDKs
    id("org.gradle.toolchains.foojay-resolver-convention").version("0.8.0")
}

rootProject.name = "SmartParking"

// Use Maven Central for resolving dependencies in each sub-project.
dependencyResolutionManagement {
    repositories {
        mavenCentral()
    }
}

// list of sub-projects
include("util", "sensor", "monitor", "mobile", "mwbot", "debug")
