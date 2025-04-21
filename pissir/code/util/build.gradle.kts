/*
 * https://docs.gradle.org/8.11.1/userguide/building_java_projects.html
 */

// Apply the application plugin to add support for building a CLI application in Java.
plugins {
    `java-library`
}

dependencies {
	// Google Gson
	implementation(libs.gson)

	// Eclipse Paho MQTT client
	implementation(libs.paho)
}

// Apply a specific Java toolchain to ease working on different environments.
java.toolchain.languageVersion.set(JavaLanguageVersion.of(21))

val jarBaseName = "util"

val appJar = projectDir.resolve("build/libs/$jarBaseName.jar")

tasks.jar {
	from(configurations.runtimeClasspath.get().map(::zipTree))
	duplicatesStrategy = DuplicatesStrategy.EXCLUDE
	archiveBaseName = jarBaseName
}
