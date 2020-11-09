# GitHub Repo README.md Dead Link Finder
A program which will search for Repos with dead links in their README.md files.

You can now check your own Repos via the web form: https://githubreporeadmelinkchecker.azurewebsites.net/

It is surprising how many active GitHub repos have bad links in their README.md files!

If you have reached this Repo page because it has found a bad link in one of your repo's README.md files then please consider showing you appreciation by giving this Repo a Star.

See an example of the program in action (it finds a Repo with a bad link in it's README.md after about 30 seconds)...
![Image](deadlink-finder-example.gif)


## How it works
Currently this program searches all of GitHub for Repos with over 100 Stars that have had a commit within the last hour.

A (user changeable) default of 25 repos are checked per run of the program.

Once a bad link is found the relevant informaiton saved in a file.

Then this information can be manually passed on, likely by creating an Issue in the affected Repo (which is probably why you might have ended up here reading this).



## If you run this program yourself
The GitHub API has a Rate Limit to throttle the amount of request from a certain source: https://developer.github.com/v3/rate_limit/

To increase this limit when using this program you can enter your Personal Access Token: https://github.com/settings/tokens

However this is optional and the program will still work without this although this limit may get hit.
