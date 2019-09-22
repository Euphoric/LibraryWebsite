docker build -t radekfalhar/librarywebsite:latest -f LibraryWebsite/Dockerfile .
docker-compose -f docker-compose.endtoendtest.yml up --build --abort-on-container-exit --exit-code-from end_to_end_test