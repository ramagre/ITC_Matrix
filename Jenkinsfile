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
                    [url: "https://github.com/ramagre/ITC_Matrix.git"]
                ]])

            }

        }

        stage('NUGET PULL') {
            steps {
                dir('.nuget') {

                    bat '''
                       C://Nuget//Nuget.exe restore "C://DATA//GitRepo//-DIGO-//ITC_Matrix.sln"
                    '''

                }

            }

        }
        
        
        stage('BUILD') {

    steps {

        bat '''
        
        C: /Windows/Microsoft.NET / Framework / v4 .0 .30319 / MSBuild.exe DIOnline / ITC_Matrix.csproj / p: DeployOnBuild = true / p: Configuration = Release / p: DebugType = Full / p: VisualStudioVersion = 15.0

        '''
        

    }

}

stage('SonarQube analysis') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat "./gradlew sonarqube"
                }
            
        }
        stage("Quality gate") {
            steps {
                waitForQualityGate abortPipeline: true
            
        }

}

    }
    }
}
