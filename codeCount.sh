echo "lines count of '.cs', '.js', '.py'"
find . -name "*.js" ! -path "*node_modules*" -o -name "*.py" -o -name "*.cs" | xargs wc -l | grep total | awk '{print $1}'
