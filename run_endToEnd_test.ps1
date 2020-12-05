docker build -t radekfalhar/librarywebsite:latest -f LibraryWebsite/Dockerfile .
docker-compose -f docker-compose.setup_cert.yml up
docker-compose -f docker-compose.endtoendtest.yml up --abort-on-container-exit --exit-code-from end_to_end_test