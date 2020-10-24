docker build -t radekfalhar/librarywebsite_test:latest -f Dockerfile-test .

docker run -v TestResults:/src/TestResults radekfalhar/librarywebsite_test unit_dotnet