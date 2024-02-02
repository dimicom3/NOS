savedcmd_/home/user/test/test.mod := printf '%s\n'   test.o | awk '!x[$$0]++ { print("/home/user/test/"$$0) }' > /home/user/test/test.mod
