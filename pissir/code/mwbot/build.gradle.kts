/*
 * https://docs.gradle.org/8.11.1/userguide/building_java_projects.html
 */

plugins {
    // Apply the application plugin to add support for building a CLI application in Java.
    application
}

dependencies {
	implementation(project(":util"))
}

// Apply a specific Java toolchain to ease working on different environments.
java.toolchain.languageVersion.set(JavaLanguageVersion.of(21))

group = "group14.pissir"

val mainClassName = "MWbot"

// Define the main class for the application.
application.mainClass = "$group.iot.mwbot.$mainClassName"

val jarBaseName = "mwbot"

val appJar = projectDir.resolve("build/libs/$jarBaseName.jar")

tasks {
	named<JavaExec>("run") {
		standardInput = System.`in`
	}

	jar {
		dependsOn(":util:jar")
		manifest.attributes["Main-Class"] = application.mainClass.get()
		from(configurations.runtimeClasspath.get().map(::zipTree))
		duplicatesStrategy = DuplicatesStrategy.EXCLUDE
		archiveBaseName = jarBaseName
		println("[GRADLE] INFO created jar file. Execute \"java -jar $appJar <true|false>\"")
	}

	register<Exec>("runJar") {
		dependsOn("jar")
		commandLine("java", "-jar", appJar)
		standardInput = System.`in`
	}
}
