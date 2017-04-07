echo "lines count of .cs and .js"
find . -name "*.?s" | xargs wc -l | grep total | awk '{print $1}'
