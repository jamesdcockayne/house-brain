if (test-path "house-brain") {
	remove-item "house-brain"
}

docker build -t house-brain . -f .\Service.Dockerfile

if ($LastExitCode -ne 0) {
	return;
}

docker save -o house-brain house-brain
scp -i "C:\house-brain-keys\house-brain-keys.pem" house-brain james@10.0.0.18:/home/james/house-brain




