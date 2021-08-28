pipeline {
    agent any

    stages {

        stage('CLEAN') {

            steps {
                cleanWs()
            }
        }

        stage('CODE CHECKOUT') {

            steps {

                checkout([$class: 'GitSCM', branches: [
                    [name: "master"]
                ], userRemoteConfigs: [
                    [url: "https://github.com/ramagre/-DIGO-.git"]
                ]])

            }

        }

        stage('NUGET PULL') {
            steps {
                dir('.nuget') {

                    bat '''
                       C://Nuget//Nuget.exe restore "C://DATA//GitRepo//DIGO//ITC_Matrix.sln"
                    '''

                }

            }

        }

    }
}
