docker build -t radekfalhar/librarywebsite_test:latest -f Dockerfile-test .
docker build -t radekfalhar/librarywebsite_test_angular:latest -f Dockerfile-test-angular .

docker run -v TestResults:/src/TestResults radekfalhar/librarywebsite_test unit_dotnet
docker run -v TestResults:/src/TestResults radekfalhar/librarywebsite_test_angular